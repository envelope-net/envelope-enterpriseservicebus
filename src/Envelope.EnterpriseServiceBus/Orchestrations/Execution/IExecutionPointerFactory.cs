﻿using Envelope.EnterpriseServiceBus.Orchestrations.Definition;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Execution;

public interface IExecutionPointerFactory
{
	ExecutionPointer BuildGenesisPointer(IOrchestrationInstance orchestrationInstance);

	ExecutionPointer? BuildNextPointer(IOrchestrationInstance orchestrationInstance, IExecutionPointer previousPointer, Guid idNextStep);

	ExecutionPointer? BuildNestedPointer(IOrchestrationInstance orchestrationInstance, IExecutionPointer previousPointer, Guid idNestedStep);
}
