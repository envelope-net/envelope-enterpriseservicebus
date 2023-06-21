using Envelope.EnterpriseServiceBus.Exchange;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Internal;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus;
using Envelope.ServiceBus.Hosts;
using Envelope.ServiceBus.Hosts.Logging;
using Envelope.ServiceBus.Messages.Resolvers;
using Envelope.Text;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration.Internal;

internal class ServiceBusOptions : IServiceBusOptions, IValidable
{
	public IServiceProvider ServiceProvider { get; }
	public ServiceBusMode ServiceBusMode { get; set; }
	public IHostInfo HostInfo { get; set; }
	public IMessageTypeResolver MessageTypeResolver { get; set; }
	public IHostLogger HostLogger { get; set; }
	public IExchangeProvider ExchangeProvider { get; set; }
	public IQueueProvider QueueProvider { get; set; }
	public Type MessageHandlerContextType { get; set; }
	public Func<IServiceProvider, IMessageHandlerContext> MessageHandlerContextFactory { get; set; }
	public IHandlerLogger HandlerLogger { get; set; }
	public IMessageHandlerResultFactory MessageHandlerResultFactory { get; set; }
	public IServiceBusLifeCycleEventManager ServiceBusLifeCycleEventManager { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public ServiceBusOptions(IServiceProvider serviceProvider)
	{
		ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		ServiceBusLifeCycleEventManager = new ServiceBusLifeCycleEventManager();
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
			.IfNull(ServiceProvider)
			.IfNull(HostInfo)
			.IfNull(MessageTypeResolver)
			.IfNull(HostLogger)
			.IfNull(ExchangeProvider)
			.IfNull(QueueProvider)
			.IfNull(MessageHandlerContextType)
			.IfNull(MessageHandlerContextFactory)
			.IfNull(HandlerLogger)
			.IfNull(MessageHandlerResultFactory)
			;

		return validationBuilder.Build();
	}

	public void LogEnvironmentInfo()
		=> HostLogger.LogEnvironmentInfo(HostInfo?.EnvironmentInfo ?? throw new InvalidOperationException($"{nameof(HostInfo)}?.{nameof(HostInfo.EnvironmentInfo)} == null"));
}
