﻿using Envelope.EnterpriseServiceBus.Exchange.Configuration;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.ServiceBus.Messages;
using Envelope.Services;
using Envelope.Trace;
using Envelope.Transactions;

namespace Envelope.EnterpriseServiceBus.Exchange.Internal;

internal class MessageBrokerHandler<TMessage> : IMessageBrokerHandler<TMessage>
	where TMessage : class, IMessage
{
	public async Task<MessageHandlerResult> HandleAsync(IExchangeMessage<TMessage> message, ExchangeContext<TMessage> exchangeContext, ITransactionController transactionController, CancellationToken cancellationToken)
	{
		var traceInfo = TraceInfo.Create(exchangeContext.ServiceBusOptions.ServiceProvider);
		var result = new ResultBuilder();

		var messageHandlerResultFactory = exchangeContext.ServiceBusOptions.MessageHandlerResultFactory;

		var queue = exchangeContext.ServiceBusOptions.QueueProvider.GetQueue<TMessage>(message.TargetQueueName!);
		if (queue == null)
		{
			result.WithInvalidOperationException(traceInfo, $"Target Queue with name '{message.TargetQueueName}' does not exists.");
			return messageHandlerResultFactory.Error(result.Build());
		}

		bool disableFaultQueue = false;
		try
		{
			var context = exchangeContext.ServiceBusOptions.QueueProvider.CreateQueueEnqueueContext(traceInfo, message);
			disableFaultQueue = context.DisableFaultQueue;
			var enqueueResult = await queue.EnqueueAsync(message.Message, context, transactionController, cancellationToken).ConfigureAwait(false);
			if (result.MergeHasError(enqueueResult))
			{
				return messageHandlerResultFactory.Error(result.Build());
			}
			else
			{
				return messageHandlerResultFactory.DeliveredInternal(context.OnMessageQueueInternal);
			}
		}
		catch (Exception ex)
		{
			var errorMessage = 
				await exchangeContext.ServiceBusOptions.HostLogger.LogErrorAsync(
					traceInfo,
					exchangeContext.ServiceBusOptions.HostInfo,
					x => x
						.ExceptionInfo(ex)
						.Detail($"{nameof(message.ExchangeName)} == {message.ExchangeName} | {nameof(message.TargetQueueName)} == {message.TargetQueueName} | MessageType = '{message.Message?.GetType().FullName}'"),
					$"{nameof(HandleAsync)}<{nameof(TMessage)}>",
					null,
					cancellationToken: default).ConfigureAwait(false);

			if (!disableFaultQueue)
			{
				try
				{
					var faultContext = exchangeContext.ServiceBusOptions.QueueProvider.CreateFaultQueueContext(traceInfo, message);
					await exchangeContext.ServiceBusOptions.QueueProvider.FaultQueue.EnqueueAsync(message.Message, faultContext, transactionController, cancellationToken).ConfigureAwait(false);
				}
				catch (Exception faultEx)
				{
					await exchangeContext.ServiceBusOptions.HostLogger.LogErrorAsync(
						traceInfo,
						exchangeContext.ServiceBusOptions.HostInfo,
						x => x
							.ExceptionInfo(faultEx)
							.Detail($"{nameof(message.ExchangeName)} == {message.ExchangeName} | {nameof(message.TargetQueueName)} == {message.TargetQueueName} | MessageType = {message.Message?.GetType().FullName} >> {nameof(exchangeContext.ServiceBusOptions.QueueProvider.FaultQueue)}.{nameof(exchangeContext.ServiceBusOptions.QueueProvider.FaultQueue.EnqueueAsync)}"),
						$"{nameof(HandleAsync)}<{nameof(TMessage)}> >> {nameof(exchangeContext.ServiceBusOptions.QueueProvider.FaultQueue)}",
						null,
						cancellationToken: default).ConfigureAwait(false);
				}
			}

			result.WithError(errorMessage);
			return messageHandlerResultFactory.AbortedInternal(result.Build());
		}
	}
}
