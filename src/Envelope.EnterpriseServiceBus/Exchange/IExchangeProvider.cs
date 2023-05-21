using Envelope.EnterpriseServiceBus.Exchange.Routing;
using Envelope.EnterpriseServiceBus.Messages.Options;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus;
using Envelope.ServiceBus.Messages;
using Envelope.Services;
using Envelope.Trace;

namespace Envelope.EnterpriseServiceBus.Exchange;

public interface IExchangeProvider
{
	/// <summary>
	/// Prepresents the falt queue, e.g. when no exchange with specific name was created,
	/// the message will be inserted to fault queue.
	/// </summary>
	IFaultQueue FaultQueue { get; }

	/// <summary>
	/// Get's all exchanges
	/// </summary>
	List<IExchange> GetAllExchanges();

	/// <summary>
	/// Get's the exchange for the specific exchange name
	/// </summary>
	IExchange? GetExchange(string exchangeName);

	/// <summary>
	/// Get's the exchange for the specific exchange name and message type
	/// </summary>
	IExchange<TMessage>? GetExchange<TMessage>(string exchangeName)
		where TMessage : class, IMessage;

	IResult<IExchangeEnqueueContext> CreateExchangeEnqueueContext(ITraceInfo traceInfo, IMessageOptions options, ExchangeType exchangeType, ServiceBusMode serviceBusMode);

	IFaultQueueContext CreateFaultQueueContext(ITraceInfo traceInfo, IMessageOptions options);
}
