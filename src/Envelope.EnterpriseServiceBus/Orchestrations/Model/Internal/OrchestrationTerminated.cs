using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class OrchestrationTerminated : LifeCycleEvent, IOrchestrationTerminated, ILifeCycleEvent, IEvent
{
	public OrchestrationTerminated(IOrchestrationInstance orchestrationInstance)
		: base(orchestrationInstance)
	{
	}
}
