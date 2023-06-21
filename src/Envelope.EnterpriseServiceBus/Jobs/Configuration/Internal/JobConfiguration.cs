using Envelope.Calendar;
using Envelope.ServiceBus.Jobs;
using Envelope.ServiceBus.Jobs.Configuration;
using Envelope.Validation;

#nullable disable

namespace Envelope.EnterpriseServiceBus.Jobs.Configuration.Internal;

internal class JobConfiguration : IJobConfiguration, IValidable
{
	public string Name { get; set; }

	public string? Description { get; set; }

	public bool Disabled { get; set; }

	public JobExecutingMode Mode { get; set; }

	public TimeSpan? DelayedStart { get; set; }

	public TimeSpan? IdleTimeout { get; set; }

	public bool CronStartImmediately { get; set; }

	public CronTimerSettings CronTimerSettings { get; set; }

	public int ExecutionEstimatedTimeInSeconds { get; set; }

	public int DeclaringAsOfflineAfterMinutesOfInactivity { get; set; }

	public Dictionary<int, string> JobExecutionOperations { get; set; }

	public List<int> AssociatedJobMessageTypes { get; set; }

	public List<IValidationMessage>? Validate(
		string? propertyPrefix = null,
		ValidationBuilder? validationBuilder = null,
		Dictionary<string, object>? globalValidationContext = null,
		Dictionary<string, object>? customValidationContext = null)
	{
		validationBuilder ??= new ValidationBuilder();
		validationBuilder.SetValidationMessages(propertyPrefix, globalValidationContext)
			.IfNullOrWhiteSpace(Name)
			.If(
				(Mode == JobExecutingMode.SequentialIntervalTimer
					|| Mode == JobExecutingMode.ExactPeriodicTimer)
					&& !IdleTimeout.HasValue)
			.If(Mode == JobExecutingMode.Cron && CronTimerSettings == null)
			.If(ExecutionEstimatedTimeInSeconds < 0)
			.If(DeclaringAsOfflineAfterMinutesOfInactivity < 0)
			.If(0 < DeclaringAsOfflineAfterMinutesOfInactivity && (DeclaringAsOfflineAfterMinutesOfInactivity * 60) < ExecutionEstimatedTimeInSeconds)
			;

		return validationBuilder.Build();
	}
}
