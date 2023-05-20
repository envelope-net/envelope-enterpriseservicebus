using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

internal class DelayStepBody : ISyncStepBody, IStepBody
{	
	public TimeSpan DelayInterval { get; set; }

	public BodyType BodyType => BodyType.Delay;

	public IExecutionResult Run(IStepExecutionContext context)
		=> ExecutionResultFactory.Delay(DelayInterval);
}
