﻿using Envelope.EnterpriseServiceBus.Orchestrations.Model;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Execution;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IExecutionPointerUpdate
{
	Guid IdExecutionPointer { get; }

	bool SetActive { get; }
	bool Active { get; }

	bool SetStatus { get; }
	PointerStatus Status { get; }

	bool SetSleepUntilUtc { get; }
	DateTime? SleepUntilUtc { get; }

	bool SetRetryCount { get; }
	int RetryCount { get; }

	bool SetStartTimeUtc { get; }
	DateTime? StartTimeUtc { get; }

	bool SetEndTimeUtc { get; }
	DateTime? EndTimeUtc { get; }

	bool SetEventName { get; }
	string? EventName { get; }

	bool SetEventKey { get; }
	string? EventKey { get; }

	bool SetEventWaitingTimeToLiveUtc { get; }
	DateTime? EventWaitingTimeToLiveUtc { get; }

	bool SetOrchestrationEvent { get; }
	OrchestrationEvent? OrchestrationEvent { get; }

	bool SetPredecessorExecutionPointerId { get; }
	Guid? PredecessorExecutionPointerId { get; }

	bool SetPredecessorExecutionPointerStartingStepId { get; }
	Guid? PredecessorExecutionPointerStartingStepId { get; }
}
