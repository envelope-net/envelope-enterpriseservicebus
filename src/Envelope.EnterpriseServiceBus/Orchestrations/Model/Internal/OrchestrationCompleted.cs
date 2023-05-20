using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class OrchestrationCompleted : LifeCycleEvent, IOrchestrationCompleted, ILifeCycleEvent, IEvent
{
	public OrchestrationCompleted(IOrchestrationInstance orchestrationInstance)
		: base(orchestrationInstance)
	{
	}
}
