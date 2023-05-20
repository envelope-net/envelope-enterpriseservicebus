using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps;

public interface ISyncOrchestrationStep<TStepBody> : IOrchestrationStep
	where TStepBody : ISyncStepBody
{
	TStepBody? ConstructStepBody(IServiceProvider serviceProvider);
}
