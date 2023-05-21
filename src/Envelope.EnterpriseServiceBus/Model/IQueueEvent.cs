using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Model;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IQueueEvent : IServiceBusEvent, IEvent
{
	DateTime EventTimeUtc { get; }

	QueueEventType QueueEventType { get; }

	IMessageQueue MessageQueue { get; }
}

public enum QueueEventType
{
	Enqueue,
	OnMessage,
	Peek,
	Remove
}
