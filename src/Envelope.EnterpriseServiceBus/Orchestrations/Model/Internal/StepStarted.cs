using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class StepStarted : StepLifeCycleEvent, IStepStarted, IStepLifeCycleEvent, ILifeCycleEvent, IEvent
{
	public StepStarted(IOrchestrationInstance orchestrationInstance, IExecutionPointer executionPointer)
		: base(orchestrationInstance, executionPointer)
	{
	}
}
