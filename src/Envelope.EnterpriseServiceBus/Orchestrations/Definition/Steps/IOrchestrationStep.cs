﻿using Envelope.EnterpriseServiceBus.ErrorHandling;
using Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IOrchestrationStep : IValidable
{
	Guid IdStep { get; }

	Type? BodyType { get; }

	string Name { get; set; }

	bool IsRootStep { get; set; }

	IOrchestrationDefinition OrchestrationDefinition { get; set; }

	IOrchestrationStep? NextStep { get; set; }

	IReadOnlyDictionary<object, IOrchestrationStep> Branches { get; }

	IOrchestrationStep? BranchController { get; set; }

	IOrchestrationStep? StartingStep { get; set; }

	bool IsStartingStep { get; }

	IErrorHandlingController? ErrorHandlingController { get; set; }

	TimeSpan? DistributedLockExpiration { get; set; }

	IStepBody? ConstructBody(IServiceProvider serviceProvider);

	IErrorHandlingController? GetErrorHandlingController();

	bool CanRetry(int retryCount);

	TimeSpan? GetRetryInterval(int retryCount);

	AssignParameters? SetInputParametersInternal { get; set; }

	AssignParameters? SetOutputParametersInternal { get; set; }
}
