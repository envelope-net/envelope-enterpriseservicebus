using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class OrchestrationResumed : LifeCycleEvent, IOrchestrationResumed, ILifeCycleEvent, IEvent
{
	public OrchestrationResumed(IOrchestrationInstance orchestrationInstance)
		: base(orchestrationInstance)
	{
	}
}
