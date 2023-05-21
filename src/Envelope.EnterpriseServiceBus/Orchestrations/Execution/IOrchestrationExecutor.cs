using Envelope.EnterpriseServiceBus.Orchestrations.Configuration;
using Envelope.EnterpriseServiceBus.Orchestrations.Logging;
using Envelope.Trace;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Execution;

public interface IOrchestrationExecutor
{
	IOrchestrationLogger OrchestrationLogger { get; }
	IOrchestrationHostOptions OrchestrationHostOptions { get; }

	Task RestartAsync(IOrchestrationInstance orchestrationInstance, ITraceInfo traceInfo);
	Task ExecuteAsync(IOrchestrationInstance orchestrationInstance, ITraceInfo traceInfo);
}
