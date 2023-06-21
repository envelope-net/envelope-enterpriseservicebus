using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.EnterpriseServiceBus.Orchestrations.Logging;
using Envelope.ServiceBus.DistributedCoordinator;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Configuration;

public class OrchestrationHostConfiguration : IOrchestrationHostConfiguration, IValidable
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public bool RegisterAsHostedService { get; set; }
	public Func<IServiceProvider, IOrchestrationRegistry> OrchestrationRegistry { get; set; }
	public Func<IServiceProvider, IExecutionPointerFactory> ExecutionPointerFactory { get; set; }
	public Func<IServiceProvider, IOrchestrationRegistry, IOrchestrationRepository> OrchestrationRepositoryFactory { get; set; }
	public Func<IServiceProvider, IDistributedLockProvider> DistributedLockProviderFactory { get; set; }
	public Func<IServiceProvider, IOrchestrationLogger> OrchestrationLogger { get; set; }
	public Func<IServiceProvider, IEventPublisher>? EventPublisherFactory { get; set; }
	public ErrorHandlerConfigurationBuilder ErrorHandlerConfigurationBuilder { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public List<IValidationMessage>? Validate(
		string? propertyPrefix = null,
		ValidationBuilder? validationBuilder = null,
		Dictionary<string, object>? globalValidationContext = null,
		Dictionary<string, object>? customValidationContext = null)
	{
		validationBuilder ??= new ValidationBuilder();
		validationBuilder.SetValidationMessages(propertyPrefix, globalValidationContext)
			.IfNull(OrchestrationRegistry)
			.IfNull(ExecutionPointerFactory)
			.IfNull(OrchestrationRepositoryFactory)
			.IfNull(DistributedLockProviderFactory)
			.IfNull(OrchestrationLogger)
			.IfNull(ErrorHandlerConfigurationBuilder)
			;

		return validationBuilder.Build();
	}
}
