﻿using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model.Internal;

internal class OrchestrationSuspended : LifeCycleEvent, IOrchestrationSuspended, ILifeCycleEvent, IEvent
{
	public SuspendSource SuspendSource { get; }

	public OrchestrationSuspended(IOrchestrationInstance orchestrationInstance, SuspendSource suspendSource)
		: base(orchestrationInstance)
	{
		SuspendSource = suspendSource;
	}
}
