using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Builder;
using Envelope.ServiceBus.ErrorHandling;

namespace Envelope.EnterpriseServiceBus.Orchestrations;

public interface IOrchestration<TData>
{
	Guid IdOrchestrationDefinition { get; }
	int Version { get; }
	string? Description { get; }
	bool IsSingleton { get; }
	bool AwaitForHandleLifeCycleEvents { get; }
	IErrorHandlingController? DefaultErrorHandling { get; }
	TimeSpan DefaultDistributedLockExpiration { get; }
	TimeSpan WorkerIdleTimeout { get; }
	void Build(OrchestrationBuilder<TData> builder);
}
