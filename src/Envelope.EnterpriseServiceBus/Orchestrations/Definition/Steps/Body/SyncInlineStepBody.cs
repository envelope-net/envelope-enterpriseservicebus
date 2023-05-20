using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

internal class SyncInlineStepBody : ISyncInlineStepBody, ISyncStepBody, IStepBody
{
	private readonly Func<IStepExecutionContext, IExecutionResult> _body;

	public BodyType BodyType => BodyType.Inline;

	public SyncInlineStepBody(Func<IStepExecutionContext, IExecutionResult> body)
	{
		_body = body ?? throw new ArgumentNullException(nameof(body));
	}

	public IExecutionResult Run(IStepExecutionContext context)
		=> _body.Invoke(context);
}
