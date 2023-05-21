using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Hosts;
using Envelope.ServiceBus.Hosts.Logging;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IEventBusOptions : IValidable
{
	IHostInfo HostInfo { get; }
	IHostLogger HostLogger { get; }
	IHandlerLogger HandlerLogger { get; }
	IMessageBodyProvider? EventBodyProvider { get; }
	IMessageHandlerResultFactory MessageHandlerResultFactory { get; }
}
