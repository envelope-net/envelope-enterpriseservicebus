using Envelope.Logging;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.Orchestrations.Execution;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class OrchestrationError : StepLifeCycleEvent, IOrchestrationError, IStepLifeCycleEvent, ILifeCycleEvent, IEvent
{
	public IErrorMessage ErrorMessage { get; set; }

	public OrchestrationError(IOrchestrationInstance orchestrationInstance, IExecutionPointer executionPointer, IErrorMessage errorMessage)
		: base(orchestrationInstance, executionPointer)
	{
		ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
	}
}
