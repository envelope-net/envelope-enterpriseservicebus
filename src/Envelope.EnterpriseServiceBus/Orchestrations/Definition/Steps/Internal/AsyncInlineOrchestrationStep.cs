using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Internal;

internal class AsyncInlineOrchestrationStep : AsyncOrchestrationStep<AsyncInlineStepBody>
{
	public Func<IStepExecutionContext, Task<IExecutionResult>> Body { get; set; }

	public AsyncInlineOrchestrationStep(Func<IStepExecutionContext, Task<IExecutionResult>> body)
		: base("AsyncInline")
	{
		Body = body ?? throw new ArgumentNullException(nameof(body));
	}

	public override AsyncInlineStepBody? ConstructStepBody(IServiceProvider serviceProvider)
		=> new(Body);
}
