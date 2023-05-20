using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Internal;

internal class EndOrchestrationStep : OrchestrationStep
{
	public override Type? BodyType => null;

	public EndOrchestrationStep(string name)
		: base(name)
	{
	}

	public override IStepBody? ConstructBody(IServiceProvider serviceProvider)
		=> null;
}
