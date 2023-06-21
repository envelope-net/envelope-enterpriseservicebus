using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Queues.Configuration;

public class QueueProviderConfiguration : IQueueProviderConfiguration, IValidable
{
	public IServiceBusOptions ServiceBusOptions { get; }

	internal Dictionary<string, Func<IServiceProvider, IMessageQueue>> MessageQueues { get; }
	Dictionary<string, Func<IServiceProvider, IMessageQueue>> IQueueProviderConfiguration.MessageQueuesInternal => MessageQueues;

	public Func<IServiceProvider, IFaultQueue> FaultQueue { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public QueueProviderConfiguration(IServiceBusOptions serviceBusOptions)
	{
		ServiceBusOptions = serviceBusOptions ?? throw new ArgumentNullException(nameof(serviceBusOptions));
		MessageQueues = new Dictionary<string, Func<IServiceProvider, IMessageQueue>>();
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public List<IValidationMessage>? Validate(
		string? propertyPrefix = null,
		ValidationBuilder? validationBuilder = null,
		Dictionary<string, object>? globalValidationContext = null,
		Dictionary<string, object>? customValidationContext = null)
	{
		validationBuilder ??= new ValidationBuilder();
		validationBuilder.SetValidationMessages(propertyPrefix, globalValidationContext)
			.IfNull(FaultQueue)
			.IfNullOrEmpty(MessageQueues)
			;

		return validationBuilder.Build();
	}
}
