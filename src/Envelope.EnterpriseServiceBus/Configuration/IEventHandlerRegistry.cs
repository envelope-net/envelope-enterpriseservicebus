using Envelope.EnterpriseServiceBus.MessageHandlers;

namespace Envelope.EnterpriseServiceBus.Configuration;

public interface IEventHandlerRegistry
{
	IMessageHandlerContext? CreateEventHandlerContext(Type eventType, IServiceProvider serviceProvider);
}
