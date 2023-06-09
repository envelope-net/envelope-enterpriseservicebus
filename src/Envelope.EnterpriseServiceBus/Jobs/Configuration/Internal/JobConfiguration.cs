﻿using Envelope.Calendar;
using Envelope.ServiceBus.Jobs;
using Envelope.ServiceBus.Jobs.Configuration;
using Envelope.Text;
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

	public List<IValidationMessage> Validate(string propertyPrefix = null, List<IValidationMessage> parentErrorBuffer = null, Dictionary<string, object> validationContext = null)
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(Name))} == null"));
		}

		if ((Mode == JobExecutingMode.SequentialIntervalTimer
			|| Mode == JobExecutingMode.ExactPeriodicTimer)
			&& !IdleTimeout.HasValue)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(IdleTimeout))} == null"));
		}

		if (Mode == JobExecutingMode.Cron && CronTimerSettings == null)
		{
			if (parentErrorBuffer == null)
				parentErrorBuffer = new List<IValidationMessage>();

			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(CronTimerSettings))} == null"));
		}

		if (ExecutionEstimatedTimeInSeconds < 0)
		{
			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(ExecutionEstimatedTimeInSeconds))} < 0"));
		}

		if (DeclaringAsOfflineAfterMinutesOfInactivity < 0)
		{
			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(DeclaringAsOfflineAfterMinutesOfInactivity))} < 0"));
		}
		
		if (0 < DeclaringAsOfflineAfterMinutesOfInactivity && (DeclaringAsOfflineAfterMinutesOfInactivity * 60) < ExecutionEstimatedTimeInSeconds)
		{
			parentErrorBuffer.Add(ValidationMessageFactory.Error($"{StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(DeclaringAsOfflineAfterMinutesOfInactivity))} < {StringHelper.ConcatIfNotNullOrEmpty(propertyPrefix, ".", nameof(ExecutionEstimatedTimeInSeconds))}"));
		}

		return parentErrorBuffer;
	}
}
