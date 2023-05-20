using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IAsyncOrchestrationStep<TAsyncStepBody> : IOrchestrationStep
	where TAsyncStepBody : IAsyncStepBody
{
	TAsyncStepBody? ConstructStepBody(IServiceProvider serviceProvider);
}
