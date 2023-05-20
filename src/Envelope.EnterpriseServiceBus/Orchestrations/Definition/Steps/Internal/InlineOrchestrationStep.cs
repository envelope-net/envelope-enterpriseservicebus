using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Internal;

internal class InlineOrchestrationStep : SyncOrchestrationStep<SyncInlineStepBody>
{
	public Func<IStepExecutionContext, IExecutionResult> Body { get; set; }

	public InlineOrchestrationStep(Func<IStepExecutionContext, IExecutionResult> body)
		: base("SyncInline")
	{
		Body = body ?? throw new ArgumentNullException(nameof(body));
	}

	public override SyncInlineStepBody? ConstructStepBody(IServiceProvider serviceProvider)
		=> new(Body);
}
