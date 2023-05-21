using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Model.Internal;

internal class QueueEvent : IQueueEvent, IServiceBusEvent, IEvent
{
	public IMessageMetadata? Message { get; }

	public DateTime EventTimeUtc { get; set; }

	public QueueEventType QueueEventType { get; }

	public IMessageQueue MessageQueue { get; set; }

	public QueueEvent(IMessageQueue messageQueue, QueueEventType queueEventType, IMessageMetadata? message)
	{
		EventTimeUtc = DateTime.UtcNow;
		QueueEventType = queueEventType;
		MessageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
		Message = message;
	}
}
