using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.Trace;

namespace Envelope.EnterpriseServiceBus;

public interface IServiceBusFactory
{
	IServiceBus Create(
		IServiceProvider serviceProvider,
		Action<ServiceBusConfigurationBuilder> configure,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default);

	IServiceBus Create(
		IServiceProvider serviceProvider,
		IServiceBusConfiguration configuration,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default);

	IServiceBus Create(
		IServiceBusOptions options,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default);
}
