﻿using Envelope.EnterpriseServiceBus.Orchestrations.Configuration;
using System.Collections.Concurrent;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Execution.Internal;

internal class OrchestrationExecutorManager
{
	private static readonly ConcurrentDictionary<Guid, IOrchestrationExecutor> _orchestrationExecutors = new();

	public static IOrchestrationExecutor GetOrCreateOrchestrationExecutor(
		Guid idOrchestrationInstance,
		IServiceProvider serviceProvider,
		IOrchestrationHostOptions options)
		=> _orchestrationExecutors.GetOrAdd(idOrchestrationInstance,
			key => new OrchestrationExecutor(serviceProvider, options));
}
