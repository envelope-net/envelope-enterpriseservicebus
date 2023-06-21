using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.EnterpriseServiceBus.Orchestrations.Logging;
using Envelope.Exceptions;
using Envelope.ServiceBus.DistributedCoordinator;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.ServiceBus.Hosts;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Configuration.Internal;

internal class OrchestrationHostOptions : IOrchestrationHostOptions, IValidable
{
	public IHostInfo HostInfo { get; }
	public string HostName { get; }
	public IOrchestrationRegistry OrchestrationRegistry { get; set; }
	public IDistributedLockProvider DistributedLockProvider { get; }
	public IErrorHandlingController ErrorHandlingController { get; }
	public Func<IServiceProvider, IEventPublisher>? EventPublisherFactory { get; }
	public Func<IServiceProvider, IOrchestrationRegistry, IOrchestrationRepository> OrchestrationRepositoryFactory { get; }
	public Func<IServiceProvider, IOrchestrationLogger> OrchestrationLogger { get; }
	public Func<IServiceProvider, IExecutionPointerFactory> ExecutionPointerFactory { get; }

	public OrchestrationHostOptions(IOrchestrationHostConfiguration config, IHostInfo hostInfo, IServiceProvider serviceProvider)
	{
		if (config == null)
			throw new ArgumentNullException(nameof(config));

		if (serviceProvider == null)
			throw new ArgumentNullException(nameof(serviceProvider));

		var error = config.Validate(nameof(OrchestrationHostConfiguration));
		if (0 < error?.Count)
			throw new ConfigurationException(error);

		HostInfo = hostInfo ?? throw new ArgumentNullException(nameof(hostInfo));
		HostName = hostInfo.HostName;
		OrchestrationRegistry = config.OrchestrationRegistry(serviceProvider);
		ExecutionPointerFactory = config.ExecutionPointerFactory;
		OrchestrationRepositoryFactory = config.OrchestrationRepositoryFactory;
		DistributedLockProvider = config.DistributedLockProviderFactory(serviceProvider);
		OrchestrationLogger = config.OrchestrationLogger;
		EventPublisherFactory = config.EventPublisherFactory;
		ErrorHandlingController = config.ErrorHandlerConfigurationBuilder.Build().BuildErrorHandlingController();
	}

	public List<IValidationMessage>? Validate(
		string? propertyPrefix = null,
		ValidationBuilder? validationBuilder = null,
		Dictionary<string, object>? globalValidationContext = null,
		Dictionary<string, object>? customValidationContext = null)
	{
		validationBuilder ??= new ValidationBuilder();
		validationBuilder.SetValidationMessages(propertyPrefix, globalValidationContext)
			.IfNull(HostInfo)
			.IfNullOrWhiteSpace(HostName)
			.IfNull(OrchestrationRegistry)
			.IfNull(ExecutionPointerFactory)
			.IfNull(OrchestrationRepositoryFactory)
			.IfNull(DistributedLockProvider)
			.IfNull(OrchestrationLogger)
			.IfNull(EventPublisherFactory)
			.IfNull(ErrorHandlingController)
			;

		return validationBuilder.Build();
	}
}
