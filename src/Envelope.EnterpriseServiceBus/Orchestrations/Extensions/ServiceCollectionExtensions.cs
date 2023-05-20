using Envelope.ServiceBus.Hosts;
using Envelope.EnterpriseServiceBus.Orchestrations.Configuration;
using Envelope.EnterpriseServiceBus.Orchestrations.Configuration.Internal;
using Envelope.EnterpriseServiceBus.Orchestrations.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Envelope.EnterpriseServiceBus.Orchestrations.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddOrchestrations(
		this IServiceCollection services,
		IHostInfo hostInfo,
		Action<OrchestrationHostConfigurationBuilder>? configure = null)
	{
		if (services.Any(x => x.ServiceType == typeof(OrchestrationHostConfiguration)))
			throw new InvalidOperationException("Orchestration services already registered");

		var orchestrationHostConfigurationBuilder = OrchestrationHostConfigurationBuilder.GetDefaultBuilder();
		configure?.Invoke(orchestrationHostConfigurationBuilder);
		var orchestrationHostConfiguration = orchestrationHostConfigurationBuilder.Build();

		services.AddSingleton<IOrchestrationHostOptions>(sp =>
		{
			var orchestrationOptions = new OrchestrationHostOptions(orchestrationHostConfiguration, hostInfo, sp);
			return orchestrationOptions;
		});

		if (orchestrationHostConfiguration.RegisterAsHostedService)
			services.AddHostedService<IOrchestrationHost>();
		else
			services.AddSingleton<IOrchestrationHost, OrchestrationHost>();

		services.AddTransient<IOrchestrationRepository>(sp =>
		{
			var options = sp.GetRequiredService<IOrchestrationHostOptions>();
			return options.OrchestrationRepositoryFactory(sp, options.OrchestrationRegistry);
		});

		return services;
	}
}
