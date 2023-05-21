using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Model;
using Envelope.Trace;

namespace Envelope.EnterpriseServiceBus.MessageHandlers;

public interface IServiceBusLifeCycleEventManager
{
	event ServiceBusEventHandler OnServiceBusEvent;

	Task PublishServiceBusEventInternalAsync(IServiceBusEvent serviceBusEvent, ITraceInfo traceInfo, IServiceBusOptions serviceBusOptions);
}



public delegate Task ServiceBusEventHandler(IServiceBusEvent serviceBusEvent, ITraceInfo traceInfo);
