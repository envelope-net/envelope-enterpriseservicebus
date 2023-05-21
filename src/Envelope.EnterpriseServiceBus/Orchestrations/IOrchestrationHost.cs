using Envelope.ServiceBus.Hosts;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Microsoft.Extensions.Hosting;

namespace Envelope.EnterpriseServiceBus.Orchestrations;

public interface IOrchestrationHost : IOrchestrationController, IHostedService
{
	IOrchestrationController OrchestrationControllerInternal { get; }

	IHostInfo HostInfo { get; }
}
