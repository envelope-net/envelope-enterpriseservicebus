using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model;

public abstract class StepLifeCycleEvent : LifeCycleEvent, IStepLifeCycleEvent, ILifeCycleEvent, IEvent
{
	public IExecutionPointer ExecutionPointer { get; }

	protected StepLifeCycleEvent(IOrchestrationInstance orchestrationInstance, IExecutionPointer executionPointer)
		: base(orchestrationInstance)
	{
		ExecutionPointer = executionPointer ?? throw new ArgumentNullException(nameof(executionPointer));
	}
}
