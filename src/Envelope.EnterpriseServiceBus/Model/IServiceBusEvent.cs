using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Model;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IServiceBusEvent : IEvent
{
	IMessageMetadata? Message { get; }
}
