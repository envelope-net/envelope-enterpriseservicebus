using Envelope.EnterpriseServiceBus.Exchange.Configuration;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Orchestrations.Model;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.EnterpriseServiceBus.Queues.Configuration;
using Envelope.ServiceBus;
using Envelope.ServiceBus.Hosts;
using Envelope.ServiceBus.Hosts.Logging;
using Envelope.ServiceBus.Messages.Resolvers;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IServiceBusConfiguration : IValidable
{
	ServiceBusMode? ServiceBusMode { get; set; }

	IHostInfo HostInfo { get; set; }

	string ServiceBusName { get; set; }

	Func<IServiceProvider, IMessageTypeResolver> MessageTypeResolver { get; set; }

	Func<IServiceProvider, IHostLogger> HostLogger { get; set; }

	Action<ExchangeProviderConfigurationBuilder> ExchangeProviderConfiguration { get; set; }

	Action<QueueProviderConfigurationBuilder> QueueProviderConfiguration { get; set; }

	Type MessageHandlerContextType { get; set; }

	Func<IServiceProvider, IMessageHandlerContext> MessageHandlerContextFactory { get; set; }

	Func<IServiceProvider, IHandlerLogger> HandlerLogger { get; set; }

	List<ServiceBusEventHandler> ServiceBusEventHandlers { get; }

	Func<IServiceProvider, IFaultQueue>? OrchestrationEventsFaultQueue { get; set; }
	
	Action<ExchangeConfigurationBuilder<OrchestrationEvent>>? OrchestrationExchange { get; set; }

	Action<MessageQueueConfigurationBuilder<OrchestrationEvent>>? OrchestrationQueue { get; set; }
}
