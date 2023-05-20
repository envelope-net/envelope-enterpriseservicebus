using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class StepSuspended : StepLifeCycleEvent, IStepSuspended, IStepLifeCycleEvent, ILifeCycleEvent, IEvent
{
	public StepSuspended(IOrchestrationInstance orchestrationInstance, IExecutionPointer executionPointer)
		: base(orchestrationInstance, executionPointer)
	{
	}
}
