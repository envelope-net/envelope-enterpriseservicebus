using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IAsyncStepBody : IStepBody
{
	Task<IExecutionResult> RunAsync(IStepExecutionContext context);
}
