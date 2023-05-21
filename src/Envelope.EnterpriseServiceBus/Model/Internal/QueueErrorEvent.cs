using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus.Messages;
using Envelope.Services;

namespace Envelope.EnterpriseServiceBus.Model.Internal;

internal class QueueErrorEvent : QueueEvent, IQueueErrorEvent, IQueueEvent, IServiceBusEvent, IEvent
{
	public IResult ErrorResult { get; }

	public QueueErrorEvent(IMessageQueue messageQueue, QueueEventType queueEventType, IMessageMetadata? message, IResult errorResult)
		: base(messageQueue, queueEventType, message)
	{
		ErrorResult = errorResult ?? throw new ArgumentNullException(nameof(errorResult));
	}
}
