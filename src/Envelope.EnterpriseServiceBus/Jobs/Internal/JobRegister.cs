﻿using Envelope.ServiceBus.Jobs;
using Envelope.ServiceBus.Jobs.Configuration;
using System.Collections.Concurrent;

namespace Envelope.EnterpriseServiceBus.Jobs.Internal;

internal class JobRegister : IJobRegister
{
	private readonly ConcurrentDictionary<string, IJob> _jobs = new();

	ConcurrentDictionary<string, IJob> IJobRegister.JobsInternal => _jobs;

	private readonly IServiceProvider _serviceProvider;
	private readonly IJobProviderConfiguration _config;

	public JobRegister(IServiceProvider serviceProvider, IJobProviderConfiguration config)
	{
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		_config = config ?? throw new ArgumentNullException(nameof(config));
	}

	public void RegisterJob(IJob job)
	{
		if (job == null)
			throw new ArgumentNullException(nameof(job));

		_jobs.AddOrUpdate(
			job.Name,
			key =>
			{
				job.InitializeInternal(_config, _serviceProvider);
				return job;
			},
			(key, existingJob) => throw new InvalidOperationException($"Job with {nameof(job.Name)} = {job.Name} already registered"));
	}
}
