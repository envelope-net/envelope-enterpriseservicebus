using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Hosts.Logging;
using Envelope.ServiceBus.Messages.Resolvers;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IMessageBusConfiguration : IValidable
{
	string MessageBusName { get; set; }

	IMessageTypeResolver MessageTypeResolver { get; set; }

	Func<IServiceProvider, IHostLogger> HostLogger { get; set; }

	Func<IServiceProvider, IHandlerLogger> HandlerLogger { get; set; }

	Func<IServiceProvider, IMessageHandlerResultFactory> MessageHandlerResultFactory { get; set; }

	IMessageBodyProvider? MessageBodyProvider { get; set; }

	List<IMessageHandlerType> MessageHandlerTypes { get; set; }

	List<IMessageHandlersAssembly> MessageHandlerAssemblies { get; set; }
}
