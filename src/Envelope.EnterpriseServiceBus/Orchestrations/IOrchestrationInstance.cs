using Envelope.EnterpriseServiceBus.Orchestrations.Definition;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.ServiceBus.DistributedCoordinator;

namespace Envelope.EnterpriseServiceBus.Orchestrations;

public interface IOrchestrationInstance : IDistributedLockKeyFactory, IDisposable, IAsyncDisposable
{
	Guid IdOrchestrationInstance { get; }
	
	string OrchestrationKey { get; }

	Guid IdOrchestrationDefinition { get; }

	bool IsSingleton { get; }

	OrchestrationStatus Status { get; set; }

	int Version { get; }

	object Data { get; }

	DateTime CreateTimeUtc { get; }

	DateTime? CompleteTimeUtc { get; }

	TimeSpan WorkerIdleTimeout { get; }

	IOrchestrationDefinition GetOrchestrationDefinition();

	void UpdateOrchestrationStatus(OrchestrationStatus status, DateTime? completeTimeUtc = null);

	Task<bool> StartOrchestrationWorkerInternalAsync();

	IOrchestrationExecutor GetExecutor();
}
