﻿using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.EnterpriseServiceBus.Orchestrations.Logging;
using Envelope.ServiceBus.DistributedCoordinator;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.ServiceBus.Hosts;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IOrchestrationHostOptions : IValidable
{
	IHostInfo HostInfo { get; }
	string HostName { get; }
	IOrchestrationRegistry OrchestrationRegistry { get; } //OrchestrationController, OrchestrationExecutor
	IDistributedLockProvider DistributedLockProvider { get; } //OrchestrationController, OrchestrationExecutor
	IErrorHandlingController ErrorHandlingController { get; }
	Func<IServiceProvider, IEventPublisher>? EventPublisherFactory { get; }
	Func<IServiceProvider, IOrchestrationRegistry, IOrchestrationRepository> OrchestrationRepositoryFactory { get; } //OrchestrationExecutor
	Func<IServiceProvider, IOrchestrationLogger> OrchestrationLogger { get; } //OrchestrationController, OrchestrationExecutor
	Func<IServiceProvider, IExecutionPointerFactory> ExecutionPointerFactory { get; } //OrchestrationController, OrchestrationExecutor
}
