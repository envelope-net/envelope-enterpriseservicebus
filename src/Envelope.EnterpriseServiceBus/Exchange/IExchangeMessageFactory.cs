using Envelope.EnterpriseServiceBus.Exchange.Configuration;
using Envelope.ServiceBus.Messages;
using Envelope.Services;
using Envelope.Trace;

namespace Envelope.EnterpriseServiceBus.Exchange;

public interface IExchangeMessageFactory<TMessage>
	where TMessage : class, IMessage
{
	IResult<List<IExchangeMessage<TMessage>>> CreateExchangeMessages(
		TMessage? message,
		IExchangeEnqueueContext context,
		ExchangeContext<TMessage> exchangeContext,
		ITraceInfo traceInfo);
}
