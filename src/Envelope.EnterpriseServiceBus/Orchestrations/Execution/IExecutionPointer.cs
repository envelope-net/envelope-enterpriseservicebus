﻿using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps;
using Envelope.EnterpriseServiceBus.Orchestrations.Model;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Execution;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IExecutionPointer
{
	Guid IdExecutionPointer { get; }

	Guid IdOrchestrationInstance { get; }

	Guid IdStep { get; }

	bool Active { get; }

	DateTime? SleepUntilUtc { get; }

	int RetryCount { get; }

	DateTime? StartTimeUtc { get; }

	DateTime? EndTimeUtc { get; }

	string? EventName { get; }

	string? EventKey { get; }

	DateTime? EventWaitingTimeToLiveUtc { get; }

	OrchestrationEvent? OrchestrationEvent { get; }
	Guid? PredecessorExecutionPointerId { get; }

	Guid? PredecessorExecutionPointerStartingStepId { get; }

	PointerStatus Status { get; }

	IOrchestrationStep GetStep();

	ExecutionPointer UpdateInternal(IExecutionPointerUpdate update);
}
