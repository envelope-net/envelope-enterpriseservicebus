﻿using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.ServiceBus.Hosts;
using Envelope.EnterpriseServiceBus.Model;
using Envelope.Trace;

namespace Envelope.EnterpriseServiceBus.MessageHandlers.Internal;

internal class ServiceBusLifeCycleEventManager : IServiceBusLifeCycleEventManager
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public event ServiceBusEventHandler OnServiceBusEvent;

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public Task PublishServiceBusEventInternalAsync(IServiceBusEvent serviceBusEvent, ITraceInfo traceInfo, IServiceBusOptions serviceBusOptions)
	{
		if (OnServiceBusEvent != null)
		{
			if (serviceBusOptions == null)
				throw new ArgumentNullException(nameof(serviceBusOptions));

			traceInfo = TraceInfo.Create(traceInfo);

			_ = Task.Run(async () =>
			{
				try
				{
					await OnServiceBusEvent.Invoke(serviceBusEvent, traceInfo).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					await serviceBusOptions.HostLogger.LogErrorAsync(
						TraceInfo.Create(traceInfo),
						serviceBusOptions.HostInfo,
						x => x.ExceptionInfo(ex),
						$"{nameof(serviceBusEvent)} type = {serviceBusEvent.GetType().FullName}",
						null,
						cancellationToken: default).ConfigureAwait(false);
				}
			});

			//try
			//{
			//	await OnServiceBusEvent.Invoke(serviceBusEvent, traceInfo).ConfigureAwait(false);
			//}
			//catch (Exception ex)
			//{
			//	await serviceBusOptions.HostLogger.LogErrorAsync(
			//		traceInfo,
			//		serviceBusOptions.HostInfo,
			//		x => x.ExceptionInfo(ex),
			//		$"{nameof(serviceBusEvent)} type = {serviceBusEvent.GetType().FullName}",
			//		null,
			//		cancellationToken: default).ConfigureAwait(false);
			//}
		}

		return Task.CompletedTask;
	}
}
