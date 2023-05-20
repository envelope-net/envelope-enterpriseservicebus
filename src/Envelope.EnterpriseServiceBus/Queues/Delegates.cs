using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Queues;

public delegate Task<MessageHandlerResult> HandleMessage<TMessage>(IQueuedMessage<TMessage> message, IMessageHandlerContext context, CancellationToken cancellationToken)
	where TMessage : class, IMessage;
