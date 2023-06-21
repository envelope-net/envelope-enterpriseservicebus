using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.ServiceBus.Messages;
using Envelope.Text;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Queues.Configuration;

public class MessageQueueConfiguration<TMessage> : IMessageQueueConfiguration<TMessage>, IValidable
		where TMessage : class, IMessage
{
	/// <inheritdoc/>
	public IServiceBusOptions ServiceBusOptions { get; }

	/// <inheritdoc/>
	public string QueueName { get; set; }

	/// <inheritdoc/>
	public QueueType QueueType { get; set; }

	/// <inheritdoc/>
	public bool IsPull { get; set; }

	/// <inheritdoc/>
	public TimeSpan? StartDelay { get; set; }

	/// <inheritdoc/>
	public TimeSpan FetchInterval { get; set; }

	/// <inheritdoc/>
	public int? MaxSize { get; set; }

	/// <inheritdoc/>
	public TimeSpan? DefaultProcessingTimeout { get; set; }

	public Func<IServiceProvider, int?, IQueue<IQueuedMessage<TMessage>>> FIFOQueue { get; set; }

	public Func<IServiceProvider, int?, IQueue<IQueuedMessage<TMessage>>> DelayableQueue { get; set; }

	/// <inheritdoc/>
	public Func<IServiceProvider, IMessageBodyProvider> MessageBodyProvider { get; set; }

	public Func<IServiceProvider, IServiceBusOptions, HandleMessage<TMessage>>? MessageHandler { get; set; }

	public Func<IServiceProvider, IErrorHandlingController>? ErrorHandling { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public MessageQueueConfiguration(IServiceBusOptions serviceBusOptions)
	{
		ServiceBusOptions = serviceBusOptions ?? throw new ArgumentNullException(nameof(serviceBusOptions));
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
			.IfNullOrWhiteSpace(QueueName)
			.If(StartDelay < TimeSpan.Zero)
			.If(FetchInterval <= TimeSpan.Zero)
			.If(MaxSize < 1)
			.If(DefaultProcessingTimeout <= TimeSpan.Zero)
			.IfNull(FIFOQueue)
			.IfNull(DelayableQueue)
			.IfNull(MessageBodyProvider)
			.If(!IsPull && MessageHandler == null)
			;

		return validationBuilder.Build();
	}
}
