﻿using Envelope.ServiceBus.Hosts.Logging;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Messages.Resolvers;
using Envelope.Text;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration;

public class EventBusConfiguration : IEventBusConfiguration, IValidable
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public string EventBusName { get; set; }
	public IMessageTypeResolver EventTypeResolver { get; set; }
	public Func<IServiceProvider, IHostLogger> HostLogger { get; set; }
	public Func<IServiceProvider, IHandlerLogger> HandlerLogger { get; set; }
	public Func<IServiceProvider, IMessageHandlerResultFactory> MessageHandlerResultFactory { get; set; }
	public IMessageBodyProvider? EventBodyProvider { get; set; }
	public List<IEventHandlerType> EventHandlerTypes { get; set; }
	public List<IEventHandlersAssembly> EventHandlerAssemblies { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public List<IValidationMessage>? Validate(string? propertyPrefix = null, List<IValidationMessage>? parentErrorBuffer = null, Dictionary<string, object>? validationContext = null)
	{
		if (string.IsNullOrWhiteSpace(EventBusName))
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(EventBusName))} == null"));
		}

		if (HostLogger == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(HostLogger))} == null"));
		}

		if (HandlerLogger == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(HandlerLogger))} == null"));
		}

		if (MessageHandlerResultFactory == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(MessageHandlerResultFactory))} == null"));
		}

		if ((EventHandlerTypes == null || EventHandlerTypes.Count == 0) && (EventHandlerAssemblies == null || EventHandlerAssemblies.Count == 0))
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error(StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", $"{nameof(EventHandlerTypes)} == null && {nameof(EventHandlerAssemblies)} == null")));
		}

		return parentErrorBuffer;
	}
}