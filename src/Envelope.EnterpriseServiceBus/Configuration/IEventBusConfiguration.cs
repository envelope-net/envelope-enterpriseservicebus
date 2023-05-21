using Envelope.ServiceBus.Hosts.Logging;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Messages.Resolvers;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IEventBusConfiguration : IValidable
{
	string EventBusName { get; set; }

	IMessageTypeResolver EventTypeResolver { get; set; }

	Func<IServiceProvider, IHostLogger> HostLogger { get; set; }

	Func<IServiceProvider, IHandlerLogger> HandlerLogger { get; set; }

	Func<IServiceProvider, IMessageHandlerResultFactory> MessageHandlerResultFactory { get; set; }

	IMessageBodyProvider? EventBodyProvider { get; set; }

	List<IEventHandlerType> EventHandlerTypes { get; set; }

	List<IEventHandlersAssembly> EventHandlerAssemblies { get; set; }
}
