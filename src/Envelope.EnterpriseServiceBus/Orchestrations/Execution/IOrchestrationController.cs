﻿using Envelope.EnterpriseServiceBus.Orchestrations.Definition;
using Envelope.EnterpriseServiceBus.Orchestrations.Model;
using Envelope.Services;
using Envelope.Trace;
using Envelope.Transactions;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Execution;

public interface IOrchestrationController
{
	IOrchestrationDefinition RegisterOrchestration<TOrchestration, TData>(ITraceInfo traceInfo)
		where TOrchestration : IOrchestration<TData>;

	event OrchestrationEventHandler OnLifeCycleEvent;

	/// <returns>returns instance id</returns>
	Task<IResult<Guid>> StartOrchestrationAsync<TData>(
		Guid idOrchestrationDefinition,
		string orchestrationKey,
		TData data,
		string lockOwner,
		ITraceInfo traceInfo,
		TimeSpan? workerIdleTimeout = null);

	/// <returns>returns instance id</returns>
	Task<IResult<Guid>> StartOrchestrationAsync<TData>(
		Guid idOrchestrationDefinition,
		string orchestrationKey,
		int? version,
		TData data,
		string lockOwner,
		ITraceInfo traceInfo,
		TimeSpan? workerIdleTimeout = null);

	Task<IOrchestrationInstance?> GetOrchestrationInstanceAsync(Guid idOrchestrationInstance, CancellationToken cancellationToken = default);

	Task<List<IOrchestrationInstance>> GetAllUnfinishedOrchestrationInstancesAsync(Guid idOrchestrationDefinition, CancellationToken cancellationToken = default);

	Task<bool?> IsCompletedOrchestrationAsync(Guid idOrchestrationInstance, CancellationToken cancellationToken = default);

	Task<List<ExecutionPointer>> GetOrchestrationExecutionPointersAsync(Guid idOrchestrationInstance, CancellationToken cancellationToken = default);

	/// <summary>
	/// Suspend the execution of a given orchestration until <see cref="ResumeOrchestrationAsync(Guid, string, ITraceInfo)"/> is called
	/// </summary>
	Task<IResult<bool>> SuspendOrchestrationAsync(Guid idOrchestrationInstance, string lockOwner, ITraceInfo traceInfo);

	/// <summary>
	/// Resume a previously suspended orchestration
	/// </summary>
	Task<IResult<bool>> ResumeOrchestrationAsync(Guid idOrchestrationInstance, string lockOwner, ITraceInfo traceInfo);

	/// <summary>
	/// Permanently terminate the exeuction of a given orchestration
	/// </summary>
	Task<IResult<bool>> TerminateOrchestrationAsync(Guid idOrchestrationInstance, string lockOwner, ITraceInfo traceInfo);

	Task PublishLifeCycleEventInternalAsync(LifeCycleEvent lifeCycleEvent, ITraceInfo traceInfo, ITransactionController transactionController);
}


public delegate Task OrchestrationEventHandler(LifeCycleEvent lifeCycleEvent, ITraceInfo traceInfo, ITransactionController transactionController);
