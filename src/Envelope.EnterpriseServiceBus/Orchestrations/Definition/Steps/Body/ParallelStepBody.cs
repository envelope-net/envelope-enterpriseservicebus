﻿using Envelope.EnterpriseServiceBus.Orchestrations.Execution;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Definition.Steps.Body;

internal class ParallelStepBody : ISyncStepBody, IStepBody
{
	public BodyType BodyType => BodyType.Parallel;

	public IExecutionResult Run(IStepExecutionContext context)
	{
		var branchIds = context.Step.Branches.Select(x => x.Value.IdStep).ToList();
		var finalizedBranchesCount = context.FinalizedBrancheIds.Count(x => branchIds.Contains(x));
		var allBranchesCompleted = branchIds.Count == finalizedBranchesCount;

		if (finalizedBranchesCount == 0)
		{
			return ExecutionResultFactory.BranchSteps(context.Step.Branches.Select(x => x.Value.IdStep).ToList());
		}
		else if (allBranchesCompleted)
		{
			return ExecutionResultFactory.NextStep();
		}
		else
		{
			return ExecutionResultFactory.Empty(); //caka sa na dokoncenie niektorej paralelnej branche
		}
	}
}
