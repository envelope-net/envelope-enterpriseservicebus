using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations;

public delegate void Assign<TStepBody, TData>(TStepBody body, TData data, IStepExecutionContext context)
	where TStepBody : IStepBody;

public delegate void AssignParameters(object body, object data, IStepExecutionContext context);