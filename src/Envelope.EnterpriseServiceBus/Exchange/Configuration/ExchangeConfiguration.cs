using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Exchange.Routing;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.ServiceBus.Messages;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Exchange.Configuration;

public class ExchangeConfiguration<TMessage> : IExchangeConfiguration<TMessage>, IValidable
		where TMessage : class, IMessage
{
	/// <inheritdoc/>
	public IServiceBusOptions ServiceBusOptions { get; }

	/// <inheritdoc/>
	public string ExchangeName { get; set; }

	/// <inheritdoc/>
	public QueueType QueueType { get; set; }

	/// <inheritdoc/>
	public TimeSpan? StartDelay { get; set; }

	/// <inheritdoc/>
	public TimeSpan FetchInterval { get; set; }

	/// <inheritdoc/>
	public int? MaxSize { get; set; }

	public Func<IServiceProvider, IExchangeMessageFactory<TMessage>> ExchangeMessageFactory { get; set; }

	public Func<IServiceProvider, IMessageBrokerHandler<TMessage>> MessageBrokerHandler { get; set; }

	public Func<IServiceProvider, int?, IQueue<IExchangeMessage<TMessage>>> FIFOQueue { get; set; }

	public Func<IServiceProvider, int?, IQueue<IExchangeMessage<TMessage>>> DelayableQueue { get; set; }

	/// <inheritdoc/>
	public Func<IServiceProvider, IMessageBodyProvider> MessageBodyProvider { get; set; }

	/// <inheritdoc/>
	public Func<IServiceProvider, IExhcangeRouter> Router { get; set; }

	public Func<IServiceProvider, IErrorHandlingController>? ErrorHandling { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	
	public ExchangeConfiguration(IServiceBusOptions serviceBusOptions)
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
			.IfNull(ServiceBusOptions)
			.IfNullOrWhiteSpace(ExchangeName)
			.If(StartDelay < TimeSpan.Zero)
			.If(FetchInterval < TimeSpan.Zero)
			.If(MaxSize < 1)
			.IfNull(ExchangeMessageFactory)
			.IfNull(MessageBrokerHandler)
			.IfNull(FIFOQueue)
			.IfNull(DelayableQueue)
			.IfNull(MessageBodyProvider)
			.IfNull(Router)
			;

		return validationBuilder.Build();
	}
}
