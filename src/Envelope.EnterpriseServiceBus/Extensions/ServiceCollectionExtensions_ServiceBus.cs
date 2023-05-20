using Envelope.EnterpriseServiceBus.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace Envelope.EnterpriseServiceBus.Extensions;

public static partial class ServiceCollectionExtensions
{
	public static IServiceCollection AddServiceBus(this IServiceCollection services)
	{
		services.AddHostedService<ServiceBusHost>();
		return services;
	}
}
