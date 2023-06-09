﻿using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Orchestrations.EventHandlers;
using Envelope.EnterpriseServiceBus.Orchestrations.Model;
using Envelope.EnterpriseServiceBus.Queues.Internal;
using Envelope.Exceptions;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Queues.Configuration;

public interface IQueueProviderConfigurationBuilder<TBuilder, TObject>
	where TBuilder : IQueueProviderConfigurationBuilder<TBuilder, TObject>
	where TObject : IQueueProviderConfiguration
{
	TBuilder Object(TObject queueProviderConfiguration);

	TObject Internal();

	TObject Build(bool finalize = false);

	TBuilder FaultQueue(Func<IServiceProvider, IFaultQueue> faultQueue, bool force = true);

	TBuilder RegisterInMemoryQueue<TMessage>(HandleMessage<TMessage>? messageHandler, bool force = true)
		where TMessage : class, IMessage;

	TBuilder RegisterQueue<TMessage>(Action<MessageQueueConfigurationBuilder<TMessage>> configure, bool force = true)
		where TMessage : class, IMessage;

	TBuilder RegisterQueue<TMessage>(string queueName, Func<IServiceProvider, IMessageQueue<TMessage>> messageQueue, bool force = true)
		where TMessage : class, IMessage;
}

public abstract class QueueProviderConfigurationBuilderBase<TBuilder, TObject> : IQueueProviderConfigurationBuilder<TBuilder, TObject>
	where TBuilder : QueueProviderConfigurationBuilderBase<TBuilder, TObject>
	where TObject : IQueueProviderConfiguration
{
	private bool _finalized = false;
	protected readonly TBuilder _builder;
	protected TObject _queueProviderConfiguration;

	protected QueueProviderConfigurationBuilderBase(TObject queueProviderConfiguration)
	{
		_queueProviderConfiguration = queueProviderConfiguration;
		_builder = (TBuilder)this;
	}

	public virtual TBuilder Object(TObject queueProviderConfiguration)
	{
		_queueProviderConfiguration = queueProviderConfiguration;
		return _builder;
	}

	public TObject Internal()
		=> _queueProviderConfiguration;

	public TObject Build(bool finalize = false)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		_finalized = finalize;

		var error = _queueProviderConfiguration.Validate(nameof(IQueueProviderConfiguration));
		if (0 < error?.Count)
			throw new ConfigurationException(error);

		return _queueProviderConfiguration;
	}

	public TBuilder FaultQueue(Func<IServiceProvider, IFaultQueue> faultQueue, bool force = true)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force || _queueProviderConfiguration.FaultQueue == null)
			_queueProviderConfiguration.FaultQueue = faultQueue;

		return _builder;
	}

	public TBuilder RegisterInMemoryQueue<TMessage>(HandleMessage<TMessage>? messageHandler, bool force = true)
		where TMessage : class, IMessage
		=> RegisterQueue(
			typeof(TMessage).FullName!,
			sp =>
			{
				var messageQueueConfiguration = MessageQueueConfigurationBuilder<TMessage>
					.GetDefaultBuilder(_queueProviderConfiguration.ServiceBusOptions, messageHandler)
					.Build();
				var context = new MessageQueueContext<TMessage>(messageQueueConfiguration, sp);
				return new MessageQueue<TMessage>(context);
			},
			force);

	public TBuilder RegisterQueue<TMessage>(Action<MessageQueueConfigurationBuilder<TMessage>> configure, bool force = true)
		where TMessage : class, IMessage
		=> configure != null
			? RegisterQueue(
				typeof(TMessage).FullName!,
				sp =>
				{
					var builder = MessageQueueConfigurationBuilder<TMessage>
						.GetDefaultBuilder(_queueProviderConfiguration.ServiceBusOptions, null);
					configure.Invoke(builder);
					var messageQueueConfiguration = builder.Build();
					var context = new MessageQueueContext<TMessage>(messageQueueConfiguration, sp);
					return new MessageQueue<TMessage>(context);
				},
				force)
			: throw new ArgumentNullException(nameof(configure));

	public TBuilder RegisterQueue<TMessage>(string queueName, Func<IServiceProvider, IMessageQueue<TMessage>> messageQueue, bool force = true)
		where TMessage : class, IMessage
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force)
			_queueProviderConfiguration.MessageQueuesInternal[queueName] = messageQueue;
		else
			_queueProviderConfiguration.MessageQueuesInternal.TryAdd(queueName, messageQueue);

		return _builder;
	}
}

public class QueueProviderConfigurationBuilder : QueueProviderConfigurationBuilderBase<QueueProviderConfigurationBuilder, IQueueProviderConfiguration>
{
	public QueueProviderConfigurationBuilder(IServiceBusOptions serviceBusOptions)
		: base(new QueueProviderConfiguration(serviceBusOptions))
	{
	}

	private QueueProviderConfigurationBuilder(QueueProviderConfiguration queueProviderConfiguration)
		: base(queueProviderConfiguration)
	{
	}

	public static implicit operator QueueProviderConfiguration?(QueueProviderConfigurationBuilder builder)
	{
		if (builder == null)
			return null;

		return builder._queueProviderConfiguration as QueueProviderConfiguration;
	}

	public static implicit operator QueueProviderConfigurationBuilder?(QueueProviderConfiguration queueProviderConfiguration)
	{
		if (queueProviderConfiguration == null)
			return null;

		return new QueueProviderConfigurationBuilder(queueProviderConfiguration);
	}

	internal static QueueProviderConfigurationBuilder GetDefaultBuilder(
		IServiceBusOptions serviceBusOptions,
		Action<MessageQueueConfigurationBuilder<OrchestrationEvent>>? configureOrchestrationQueue = null,
		Func<IServiceProvider, IFaultQueue>? orchestrationEventsFaultQueue = null)
		=> configureOrchestrationQueue != null
			? new QueueProviderConfigurationBuilder(serviceBusOptions)
				.RegisterQueue(configureOrchestrationQueue)
				.FaultQueue(orchestrationEventsFaultQueue ?? (sp => new DroppingFaultQueue()))
			: new QueueProviderConfigurationBuilder(serviceBusOptions)
				.RegisterInMemoryQueue<OrchestrationEvent>(OrchestrationEventHandler.HandleMessageAsync, true)
				.FaultQueue(sp => new DroppingFaultQueue());
}
