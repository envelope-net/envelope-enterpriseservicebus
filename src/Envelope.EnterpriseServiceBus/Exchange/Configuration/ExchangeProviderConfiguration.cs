using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Exchange.Configuration;

public class ExchangeProviderConfiguration : IExchangeProviderConfiguration, IValidable
{
	public IServiceBusOptions ServiceBusOptions { get; }

	internal Dictionary<string, Func<IServiceProvider, IExchange>> Exchanges { get; }
	Dictionary<string, Func<IServiceProvider, IExchange>> IExchangeProviderConfiguration.ExchangesInternal => Exchanges;

	public Func<IServiceProvider, IFaultQueue> FaultQueue { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public ExchangeProviderConfiguration(IServiceBusOptions serviceBusOptions)
	{
		ServiceBusOptions = serviceBusOptions ?? throw new ArgumentNullException(nameof(serviceBusOptions));
		Exchanges = new Dictionary<string, Func<IServiceProvider, IExchange>>();
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public List<IValidationMessage>? Validate(
		string? propertyPrefix = null,
		ValidationBuilder? validationBuilder = null,
		Dictionary<string, object>? globalValidationContext = null,
		Dictionary<string, object>? customValidationContext = null)
	{
		validationBuilder ??= new ValidationBuilder();
		validationBuilder.SetValidationMessages(propertyPrefix, globalValidationContext)
			.IfNull(FaultQueue)
			.IfNullOrEmpty(Exchanges)
			;

		return validationBuilder.Build();
	}
}
