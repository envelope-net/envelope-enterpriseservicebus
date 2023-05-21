using Envelope.EnterpriseServiceBus.Exchange.Configuration;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.ServiceBus.Messages;
using Envelope.Transactions;

namespace Envelope.EnterpriseServiceBus.Exchange;

public interface IMessageBrokerHandler<TMessage>
	where TMessage : class, IMessage
{
	Task<MessageHandlerResult> HandleAsync(
		IExchangeMessage<TMessage> message,
		ExchangeContext<TMessage> _exchangeContext,
		ITransactionController transactionController,
		CancellationToken cancellationToken);
}
