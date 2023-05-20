using Envelope.EnterpriseServiceBus.Messages.Internal;
using Envelope.ServiceBus.Messages;
using Envelope.Trace;

namespace Envelope.EnterpriseServiceBus.Messages;

public static class JobMessageFactory
{
	public static IJobMessage Create(
		ITraceInfo traceInfo,
		int jobMessageTypeId,
		Guid? taskCorrelationId = null,
		int priority = 100,
		DateTime? timeToLive = null,
		string? entityName = null,
		Guid? entityId = null,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null)
	{
		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		var id = Guid.NewGuid();
		var nowUtc = DateTime.UtcNow;
		var jobMessage = new JobMessage()
		{
			Id = id,
			JobMessageTypeId = jobMessageTypeId,
			CreatedUtc = nowUtc,
			LastUpdatedUtc = nowUtc,
			CreatedTraceInfo = traceInfo,
			LastUpdatedTraceInfo = traceInfo,
			TaskCorrelationId = taskCorrelationId ?? id,
			Priority = priority,
			TimeToLive = timeToLive,
			DeletedUtc = null,
			RetryCount = 0,
			DelayedToUtc = null,
			LastDelay = null,
			Status = (int)JobMessageStatus.Idle,
			LastResumedUtc = null,
			EntityName = entityName,
			EntityId = entityId,
			Properties = properties,
			Detail = detail,
			IsDetailJson = isDetailJson ?? false
		};

		return jobMessage;
	}

	private static JobMessage CloneInternal(IJobMessage message)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		var jobMessage = new JobMessage()
		{
			Id = message.Id,
			JobMessageTypeId = message.JobMessageTypeId,
			CreatedUtc = message.CreatedUtc,
			LastUpdatedUtc = message.LastUpdatedUtc,
			CreatedTraceInfo = message.CreatedTraceInfo,
			LastUpdatedTraceInfo = message.LastUpdatedTraceInfo,
			TaskCorrelationId = message.TaskCorrelationId,
			Priority = message.Priority,
			TimeToLive = message.TimeToLive,
			DeletedUtc = message.DeletedUtc,
			RetryCount = message.RetryCount,
			DelayedToUtc = message.DelayedToUtc,
			LastDelay = message.LastDelay,
			Status = message.Status,
			LastResumedUtc = message.LastResumedUtc,
			EntityName = message.EntityName,
			EntityId = message.EntityId,
			Properties = message.Properties,
			Detail = message.Detail,
			IsDetailJson = message.IsDetailJson
		};

		return jobMessage;
	}

	public static IJobMessage Clone(IJobMessage message)
		=> CloneInternal(message);

	public static bool CanComplete(IJobMessage message)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		return message.Status == (int)JobMessageStatus.Idle
			|| message.Status == (int)JobMessageStatus.Error;
	}

	public static IJobMessage? CreateCompletedMessage(
		IJobMessage message,
		ITraceInfo traceInfo,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (!CanComplete(message))
			return null;

		var clone = CloneInternal(message);

		clone.LastUpdatedUtc = DateTime.UtcNow;
		clone.LastUpdatedTraceInfo = traceInfo;
		clone.Status = (int)JobMessageStatus.Completed;

		if (properties != null)
			clone.Properties = properties;

		if (detail != null)
			clone.Detail = detail;

		if (isDetailJson.HasValue)
			clone.IsDetailJson = isDetailJson.Value;

		return clone;
	}

	public static IJobMessage? CreateErrorMessage(
		IJobMessage message,
		ITraceInfo traceInfo,
		DateTime? delayedToUtc,
		TimeSpan? delay,
		int maxRetryCount,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if ((delayedToUtc.HasValue && !delay.HasValue)
			|| (!delayedToUtc.HasValue && delay.HasValue))
			throw new InvalidOperationException($"Both {nameof(delayedToUtc)} and {nameof(delay)} must be set or null.");

		if (message.Status == (int)JobMessageStatus.Suspended
			|| message.Status == (int)JobMessageStatus.Deleted)
			return null;

		var clone = CloneInternal(message);

		clone.LastUpdatedUtc = DateTime.UtcNow;
		clone.LastUpdatedTraceInfo = traceInfo;
		clone.RetryCount++;

		if (clone.RetryCount < maxRetryCount)
		{
			clone.DelayedToUtc = delayedToUtc;
			clone.LastDelay = delay;
			clone.Status = (int)JobMessageStatus.Error;
		}
		else
		{
			clone.Status = (int)JobMessageStatus.Suspended;
		}

		if (properties != null)
			clone.Properties = properties;

		if (detail != null)
			clone.Detail = detail;

		if (isDetailJson.HasValue)
			clone.IsDetailJson = isDetailJson.Value;

		return clone;
	}

	public static bool CanSuspend(IJobMessage message)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		return message.Status == (int)JobMessageStatus.Idle
			|| message.Status == (int)JobMessageStatus.Error;
	}

	public static IJobMessage? CreateSuspendedMessage(
		IJobMessage message,
		ITraceInfo traceInfo,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (!CanSuspend(message))
			return null;

		var clone = CloneInternal(message);

		clone.LastUpdatedUtc = DateTime.UtcNow;
		clone.LastUpdatedTraceInfo = traceInfo;
		clone.Status = (int)JobMessageStatus.Suspended;

		if (properties != null)
			clone.Properties = properties;

		if (detail != null)
			clone.Detail = detail;

		if (isDetailJson.HasValue)
			clone.IsDetailJson = isDetailJson.Value;

		return clone;
	}

	public static bool CanResume(IJobMessage message)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		return message.Status == (int)JobMessageStatus.Suspended;
	}

	public static IJobMessage? CreateResumedMessage(
		IJobMessage message,
		ITraceInfo traceInfo,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (!CanResume(message))
			return null;

		var clone = CloneInternal(message);

		var nowUtc = DateTime.UtcNow;
		clone.LastUpdatedUtc = nowUtc;
		clone.LastUpdatedTraceInfo = traceInfo;
		clone.LastResumedUtc = nowUtc;
		clone.Status = (int)JobMessageStatus.Idle;

		clone.RetryCount = 0;
		clone.DelayedToUtc = null;
		clone.LastDelay = null;

		if (properties != null)
			clone.Properties = properties;

		if (detail != null)
			clone.Detail = detail;

		if (isDetailJson.HasValue)
			clone.IsDetailJson = isDetailJson.Value;

		return clone;
	}

	public static bool CanDelete(IJobMessage message)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		return message.Status != (int)JobMessageStatus.Deleted;
	}

	public static IJobMessage? CreateDeletedMessage(
		IJobMessage message,
		ITraceInfo traceInfo,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (!CanDelete(message))
			return null;

		var clone = CloneInternal(message);

		var nowUtc = DateTime.UtcNow;
		clone.LastUpdatedUtc = nowUtc;
		clone.LastUpdatedTraceInfo = traceInfo;
		clone.DeletedUtc = nowUtc;
		clone.Status = (int)JobMessageStatus.Deleted;

		if (properties != null)
			clone.Properties = properties;

		if (detail != null)
			clone.Detail = detail;

		if (isDetailJson.HasValue)
			clone.IsDetailJson = isDetailJson.Value;

		return clone;
	}

	public static IJobMessage? CreateArchivedMessage(
		IJobMessage message,
		ITraceInfo traceInfo,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null)
	{
		if (message == null)
			throw new ArgumentNullException(nameof(message));

		if (traceInfo == null)
			throw new ArgumentNullException(nameof(traceInfo));

		if (!CanDelete(message))
			return null;

		var clone = CloneInternal(message);

		var nowUtc = DateTime.UtcNow;
		clone.LastUpdatedUtc = nowUtc;
		clone.LastUpdatedTraceInfo = traceInfo;

		if (properties != null)
			clone.Properties = properties;

		if (detail != null)
			clone.Detail = detail;

		if (isDetailJson.HasValue)
			clone.IsDetailJson = isDetailJson.Value;

		return clone;
	}
}
