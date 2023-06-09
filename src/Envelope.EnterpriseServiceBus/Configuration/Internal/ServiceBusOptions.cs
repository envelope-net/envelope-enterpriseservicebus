﻿using Envelope.EnterpriseServiceBus.Exchange;
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

	public List<IValidationMessage>? Validate(string? propertyPrefix = null, List<IValidationMessage>? parentErrorBuffer = null, Dictionary<string, object>? validationContext = null)
	{
		if (ServiceProvider == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(ServiceProvider))} == null"));
		}

		if (HostInfo == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(HostInfo))} == null"));
		}

		if (MessageTypeResolver == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(MessageTypeResolver))} == null"));
		}

		if (HostLogger == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(HostLogger))} == null"));
		}

		if (ExchangeProvider == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(ExchangeProvider))} == null"));
		}

		if (QueueProvider == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(QueueProvider))} == null"));
		}

		if (MessageHandlerContextType == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(MessageHandlerContextType))} == null"));
		}

		if (MessageHandlerContextFactory == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(MessageHandlerContextFactory))} == null"));
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

		return parentErrorBuffer;
	}

	public void LogEnvironmentInfo()
		=> HostLogger.LogEnvironmentInfo(HostInfo?.EnvironmentInfo ?? throw new InvalidOperationException($"{nameof(HostInfo)}?.{nameof(HostInfo.EnvironmentInfo)} == null"));
}
