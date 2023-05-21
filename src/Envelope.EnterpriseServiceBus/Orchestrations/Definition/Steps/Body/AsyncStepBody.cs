using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

public abstract class AsyncStepBody : IAsyncStepBody
{
	public BodyType BodyType => BodyType.Custom;

	public abstract Task<IExecutionResult> RunAsync(IStepExecutionContext context);
}
