using Envelope.ServiceBus.Hosts;
using Envelope.ServiceBus.Jobs;
using Envelope.ServiceBus.Jobs.Configuration;
using Envelope.ServiceBus.Jobs.Logging;
using Envelope.ServiceBus.Queries;
using Envelope.ServiceBus.Writers;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Jobs.Configuration.Internal;

internal class JobProviderConfiguration : IJobProviderConfiguration, IValidable
{
	public IHostInfo HostInfoInternal { get; set; }

	internal IJobRegister JobRegister { get; set; }

	IJobRegister IJobProviderConfiguration.JobRegisterInternal
	{
		get { return JobRegister; }
		set { JobRegister = value; }
	}

	public Func<IServiceProvider, IJobRepository> JobRepository { get; set; }

	public Func<IServiceProvider, IJobLogger> JobLogger { get; set; }

	public Func<IServiceProvider, IServiceBusReader> ServiceBusReader { get; set; }

	public Func<IServiceProvider, IJobMessageWriter> JobMessageWriter { get; set; }

	public List<IValidationMessage>? Validate(
		string? propertyPrefix = null,
		ValidationBuilder? validationBuilder = null,
		Dictionary<string, object>? globalValidationContext = null,
		Dictionary<string, object>? customValidationContext = null)
	{
		validationBuilder ??= new ValidationBuilder();
		validationBuilder.SetValidationMessages(propertyPrefix, globalValidationContext)
			.IfNull(HostInfoInternal)
			.IfNull(JobRepository)
			.IfNull(JobLogger)
			.IfNull(ServiceBusReader)
			.IfNull(JobMessageWriter)
			;

		return validationBuilder.Build();
	}
}
