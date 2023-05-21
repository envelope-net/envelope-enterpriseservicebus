using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Exchange.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IExchangeProviderConfiguration : IValidable
{
	IServiceBusOptions ServiceBusOptions { get; }

	Dictionary<string, Func<IServiceProvider, IExchange>> ExchangesInternal { get; }

	Func<IServiceProvider, IFaultQueue> FaultQueue { get; set; }
}
