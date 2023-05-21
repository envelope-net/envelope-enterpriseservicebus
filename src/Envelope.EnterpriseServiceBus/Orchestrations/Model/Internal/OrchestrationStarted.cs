using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class OrchestrationStarted : LifeCycleEvent, IOrchestrationStarted, ILifeCycleEvent, IEvent
{
	public OrchestrationStarted(IOrchestrationInstance orchestrationInstance)
		: base(orchestrationInstance)
	{
	}
}
