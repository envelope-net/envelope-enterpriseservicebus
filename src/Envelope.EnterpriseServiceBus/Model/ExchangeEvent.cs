using Envelope.EnterpriseServiceBus.Exchange;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Model;

public class ExchangeEvent : IExchangeEvent, IServiceBusEvent, IEvent
{
	public IMessageMetadata? Message { get; }

	public DateTime EventTimeUtc { get; set; }

	public ExchangeEventType ExchangeEventType { get; }

	public IExchange Exchange { get; set; }

	public ExchangeEvent(IExchange exchange, ExchangeEventType exchangeEventType, IMessageMetadata? message)
	{
		EventTimeUtc = DateTime.UtcNow;
		ExchangeEventType = exchangeEventType;
		Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
		Message = message;
	}
}
