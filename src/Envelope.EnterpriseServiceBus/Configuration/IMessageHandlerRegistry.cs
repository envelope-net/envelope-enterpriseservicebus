using Envelope.EnterpriseServiceBus.MessageHandlers;

namespace Envelope.EnterpriseServiceBus.Configuration;

public interface IMessageHandlerRegistry
{
	IMessageHandlerContext? CreateMessageHandlerContext(Type messageType, IServiceProvider serviceProvider);
}
