﻿using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

internal class IfElseStepBody : ISyncStepBody, IStepBody
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	
	public Func<IStepExecutionContext, bool> Condition { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public BodyType BodyType => BodyType.IfElse;

	public IExecutionResult Run(IStepExecutionContext context)
	{
		var branchIds = context.Step.Branches.Select(x => x.Value.IdStep).ToList();
		var finalizedBranchesCount = context.FinalizedBrancheIds.Count(x => branchIds.Contains(x));

		if (0 < finalizedBranchesCount)
			return ExecutionResultFactory.NextStep();

		if (Condition(context))
			return ExecutionResultFactory.BranchSteps(new List<Guid> { context.Step.Branches[true].IdStep });
		else
			return ExecutionResultFactory.BranchSteps(new List<Guid> { context.Step.Branches[false].IdStep });
	}
}
