using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition;

public interface IReadOnlyOrchestrationStepCollection : IReadOnlyCollection<IOrchestrationStep>
{
	IOrchestrationStep? FindById(Guid idStep);
}
