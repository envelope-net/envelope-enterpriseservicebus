﻿using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.DistributedCoordinator.Internal;
using Envelope.EnterpriseServiceBus.Orchestrations.Configuration.Internal;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution.Internal;
using Envelope.EnterpriseServiceBus.Orchestrations.Internal;
using Envelope.EnterpriseServiceBus.Orchestrations.Logging;
using Envelope.Exceptions;
using Envelope.ServiceBus.DistributedCoordinator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Configuration;

public interface IOrchestrationHostConfigurationBuilder<TBuilder, TObject>
	where TBuilder : IOrchestrationHostConfigurationBuilder<TBuilder, TObject>
	where TObject : IOrchestrationHostConfiguration
{
	TBuilder Object(TObject orchestrationHostConfiguration);

	TObject Build(bool finalize = false);

	TBuilder RegisterAsHostedService(bool asHostedService);

	TBuilder OrchestrationRegistry(Func<IServiceProvider, IOrchestrationRegistry> orchestrationRegistry, bool force = true);

	TBuilder ExecutionPointerFactory(Func<IServiceProvider, IExecutionPointerFactory> executionPointerFactory, bool force = true);

	TBuilder OrchestrationRepositoryFactory(Func<IServiceProvider, IOrchestrationRegistry, IOrchestrationRepository> orchestrationRepositoryFactory, bool force = true);

	TBuilder DistributedLockProviderFactory(Func<IServiceProvider, IDistributedLockProvider> distributedLockProviderFactory, bool force = true);

	TBuilder OrchestrationLogger(Func<IServiceProvider, IOrchestrationLogger> orchestrationLogger, bool force = true);

	TBuilder EventPublisherFactory(Func<IServiceProvider, IEventPublisher> eventPublisherFactory, bool force = true);

	TBuilder ErrorHandlerConfigurationBuilder(ErrorHandlerConfigurationBuilder errorHandlerConfigurationBuilder, bool force = true);
}

public abstract class OrchestrationHostConfigurationBuilderBase<TBuilder, TObject> : IOrchestrationHostConfigurationBuilder<TBuilder, TObject>
	where TBuilder : OrchestrationHostConfigurationBuilderBase<TBuilder, TObject>
	where TObject : IOrchestrationHostConfiguration
{
	private bool _finalized = false;
	protected readonly TBuilder _builder;
	protected TObject _orchestrationHostConfiguration;

	protected OrchestrationHostConfigurationBuilderBase(TObject orchestrationHostConfiguration)
	{
		_orchestrationHostConfiguration = orchestrationHostConfiguration;
		_builder = (TBuilder)this;
	}

	public virtual TBuilder Object(TObject orchestrationHostConfiguration)
	{
		_orchestrationHostConfiguration = orchestrationHostConfiguration;
		return _builder;
	}

	public TObject Build(bool finalize = false)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		_finalized = finalize;

		var error = _orchestrationHostConfiguration.Validate(nameof(IOrchestrationHostConfiguration));
		if (0 < error?.Count)
			throw new ConfigurationException(error);

		return _orchestrationHostConfiguration;
	}

	public TBuilder RegisterAsHostedService(bool asHostedService)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		_orchestrationHostConfiguration.RegisterAsHostedService = asHostedService;
		return _builder;
	}

	public TBuilder OrchestrationRegistry(Func<IServiceProvider, IOrchestrationRegistry> orchestrationRegistry, bool force = true)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force || _orchestrationHostConfiguration.OrchestrationRegistry == null)
			_orchestrationHostConfiguration.OrchestrationRegistry = orchestrationRegistry;

		return _builder;
	}

	public TBuilder ExecutionPointerFactory(Func<IServiceProvider, IExecutionPointerFactory> executionPointerFactory, bool force = true)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force || _orchestrationHostConfiguration.ExecutionPointerFactory == null)
			_orchestrationHostConfiguration.ExecutionPointerFactory = executionPointerFactory;

		return _builder;
	}

	public TBuilder OrchestrationRepositoryFactory(Func<IServiceProvider, IOrchestrationRegistry, IOrchestrationRepository> orchestrationRepositoryFactory, bool force = true)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force || _orchestrationHostConfiguration.OrchestrationRepositoryFactory == null)
			_orchestrationHostConfiguration.OrchestrationRepositoryFactory = orchestrationRepositoryFactory;

		return _builder;
	}

	public TBuilder DistributedLockProviderFactory(Func<IServiceProvider, IDistributedLockProvider> distributedLockProviderFactory, bool force = true)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force || _orchestrationHostConfiguration.DistributedLockProviderFactory == null)
			_orchestrationHostConfiguration.DistributedLockProviderFactory = distributedLockProviderFactory;

		return _builder;
	}

	public TBuilder OrchestrationLogger(Func<IServiceProvider, IOrchestrationLogger> orchestrationLogger, bool force = true)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force || _orchestrationHostConfiguration.OrchestrationLogger == null)
			_orchestrationHostConfiguration.OrchestrationLogger = orchestrationLogger;

		return _builder;
	}

	public TBuilder EventPublisherFactory(Func<IServiceProvider, IEventPublisher> eventPublisherFactory, bool force = true)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force || _orchestrationHostConfiguration.EventPublisherFactory == null)
			_orchestrationHostConfiguration.EventPublisherFactory = eventPublisherFactory;

		return _builder;
	}

	public TBuilder ErrorHandlerConfigurationBuilder(ErrorHandlerConfigurationBuilder errorHandlerConfigurationBuilder, bool force = true)
	{
		if (_finalized)
			throw new ConfigurationException("The builder was finalized");

		if (force || _orchestrationHostConfiguration.ErrorHandlerConfigurationBuilder == null)
			_orchestrationHostConfiguration.ErrorHandlerConfigurationBuilder = errorHandlerConfigurationBuilder;

		return _builder;
	}
}

public class OrchestrationHostConfigurationBuilder : OrchestrationHostConfigurationBuilderBase<OrchestrationHostConfigurationBuilder, IOrchestrationHostConfiguration>
{
	public OrchestrationHostConfigurationBuilder()
		: base(new OrchestrationHostConfiguration())
	{
	}

	public OrchestrationHostConfigurationBuilder(OrchestrationHostConfiguration orchestrationHostConfiguration)
		: base(orchestrationHostConfiguration)
	{
	}

	public static implicit operator OrchestrationHostConfiguration?(OrchestrationHostConfigurationBuilder builder)
	{
		if (builder == null)
			return null;

		return builder._orchestrationHostConfiguration as OrchestrationHostConfiguration;
	}

	public static implicit operator OrchestrationHostConfigurationBuilder?(OrchestrationHostConfiguration orchestrationHostConfiguration)
	{
		if (orchestrationHostConfiguration == null)
			return null;

		return new OrchestrationHostConfigurationBuilder(orchestrationHostConfiguration);
	}

	internal static OrchestrationHostConfigurationBuilder GetDefaultBuilder()
		=> new OrchestrationHostConfigurationBuilder()
			//.RegisterAsHostedService(false)
			.OrchestrationRegistry(sp => new OrchestrationRegistry())
			.ExecutionPointerFactory(sp => new ExecutionPointerFactory())
			.OrchestrationRepositoryFactory((sp, registry) => new InMemoryOrchestrationRepository())
			.DistributedLockProviderFactory(sp => new InMemoryLockProvider())
			.OrchestrationLogger(sp => new DefaultOrchestrationLogger(sp.GetRequiredService<ILogger<DefaultOrchestrationLogger>>()))
			//.EventPublisherFactory(sp => sp.GetRequiredService<IEventPublisher>())
			.ErrorHandlerConfigurationBuilder(Envelope.EnterpriseServiceBus.Configuration.ErrorHandlerConfigurationBuilder.GetDefaultBuilder());
}
