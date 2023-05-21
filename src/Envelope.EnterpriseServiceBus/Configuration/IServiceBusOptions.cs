using Envelope.EnterpriseServiceBus.Exchange;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus;
using Envelope.ServiceBus.Hosts;
using Envelope.ServiceBus.Hosts.Logging;
using Envelope.ServiceBus.Messages.Resolvers;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IServiceBusOptions : IValidable
{
	IServiceProvider ServiceProvider { get; }
	ServiceBusMode ServiceBusMode { get; }
	IHostInfo HostInfo { get; }
	IMessageTypeResolver MessageTypeResolver { get; }
	IHostLogger HostLogger { get; }
	IExchangeProvider ExchangeProvider { get; }
	IQueueProvider QueueProvider { get; }
	Type MessageHandlerContextType { get; }
	Func<IServiceProvider, IMessageHandlerContext> MessageHandlerContextFactory { get; }
	IHandlerLogger HandlerLogger { get; }
	IMessageHandlerResultFactory MessageHandlerResultFactory { get; }
	IServiceBusLifeCycleEventManager ServiceBusLifeCycleEventManager { get; }

	void LogEnvironmentInfo();
}
