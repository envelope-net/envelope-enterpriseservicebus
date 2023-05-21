using Envelope.EnterpriseServiceBus.Orchestrations.Definition;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Configuration;

public interface IOrchestrationRegistry
{
	void RegisterOrchestration(IOrchestrationDefinition orchestrationDefinition);

	IOrchestrationDefinition RegisterOrchestration<TData>(IOrchestration<TData> orchestration);

	IOrchestrationDefinition? GetDefinition(Guid idOrchestrationDefinition, int? version = null);

	bool IsRegistered(Guid idOrchestrationDefinition, int version);

	IEnumerable<IOrchestrationDefinition> GetAllDefinitions();
}
