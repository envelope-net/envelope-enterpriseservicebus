using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

public abstract class SyncStepBody : ISyncStepBody
{
	public BodyType BodyType => BodyType.Custom;

	public abstract IExecutionResult Run(IStepExecutionContext context);
}
