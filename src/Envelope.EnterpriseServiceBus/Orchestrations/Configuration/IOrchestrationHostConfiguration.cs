using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.EnterpriseServiceBus.Orchestrations.Logging;
using Envelope.ServiceBus.DistributedCoordinator;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IOrchestrationHostConfiguration : IValidable
{
	bool RegisterAsHostedService { get; set; }
	Func<IServiceProvider, IOrchestrationRegistry> OrchestrationRegistry { get; set; }
	Func<IServiceProvider, IExecutionPointerFactory> ExecutionPointerFactory { get; set; }
	Func<IServiceProvider, IOrchestrationRegistry, IOrchestrationRepository> OrchestrationRepositoryFactory { get; set; }
	Func<IServiceProvider, IDistributedLockProvider> DistributedLockProviderFactory { get; set; }
	Func<IServiceProvider, IOrchestrationLogger> OrchestrationLogger { get; set; }
	Func<IServiceProvider, IEventPublisher>? EventPublisherFactory { get; set; }
	ErrorHandlerConfigurationBuilder ErrorHandlerConfigurationBuilder { get; set; }
}
