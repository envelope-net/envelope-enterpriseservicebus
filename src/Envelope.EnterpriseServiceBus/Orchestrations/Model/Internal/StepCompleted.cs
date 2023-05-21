using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class StepCompleted : StepLifeCycleEvent, IStepCompleted, IStepLifeCycleEvent, ILifeCycleEvent, IEvent
{
	public StepCompleted(IOrchestrationInstance orchestrationInstance, IExecutionPointer executionPointer)
		: base(orchestrationInstance, executionPointer)
	{
	}
}
