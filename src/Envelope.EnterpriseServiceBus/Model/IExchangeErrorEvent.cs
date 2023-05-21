using Envelope.ServiceBus.Messages;
using Envelope.Services;

namespace Envelope.EnterpriseServiceBus.Model;

public interface IExchangeErrorEvent : IExchangeEvent, IServiceBusEvent, IEvent
{
	IResult ErrorResult { get; }
}
