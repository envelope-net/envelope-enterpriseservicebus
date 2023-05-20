using Envelope.EnterpriseServiceBus.Exchange.Routing;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus.Messages;
using Envelope.Services;
using Envelope.Trace;
using Envelope.Transactions;

namespace Envelope.EnterpriseServiceBus.Exchange;

public interface IExchange : IQueueInfo, IDisposable, IAsyncDisposable
{
	ExchangeType ExchangeType { get; }

	Task OnMessageInternalAsync(ITraceInfo traceInfo, CancellationToken cancellationToken);
}

public interface IExchange<TMessage> : IExchange, IQueueInfo, IDisposable, IAsyncDisposable
	where TMessage : class, IMessage
{
	/// <summary>
	/// Enqueue the new message
	/// </summary>
	Task<IResult<List<Guid>>> EnqueueAsync(TMessage? message, IExchangeEnqueueContext context, ITransactionController transactionController, CancellationToken cancellationToken);

	Task<IResult<IExchangeMessage<TMessage>?>> TryPeekAsync(ITraceInfo traceInfo, ITransactionController transactionController, CancellationToken cancellationToken);

	Task<IResult> TryRemoveAsync(IExchangeMessage<TMessage> message, ITraceInfo traceInfo, ITransactionController transactionController, CancellationToken cancellationToken);
}
