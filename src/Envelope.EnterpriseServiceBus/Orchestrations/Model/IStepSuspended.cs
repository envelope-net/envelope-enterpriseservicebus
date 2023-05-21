﻿using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Model;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IStepSuspended : IStepLifeCycleEvent, ILifeCycleEvent, IEvent
{
}
