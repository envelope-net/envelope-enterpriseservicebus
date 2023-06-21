using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Hosts;
using Envelope.ServiceBus.Hosts.Logging;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration.Internal;

internal class MessageBusOptions : IMessageBusOptions, IValidable
{
	public IHostInfo HostInfo { get; set; }
	public IHostLogger HostLogger { get; set; }
	public IHandlerLogger HandlerLogger { get; set; }
	public IMessageBodyProvider? MessageBodyProvider { get; set; }
	public IMessageHandlerResultFactory MessageHandlerResultFactory { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public MessageBusOptions()
	{
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
			.IfNull(HostInfo)
			.IfNull(HostLogger)
			.IfNull(HandlerLogger)
			.IfNull(MessageHandlerResultFactory)
			;

		return validationBuilder.Build();
	}
}
