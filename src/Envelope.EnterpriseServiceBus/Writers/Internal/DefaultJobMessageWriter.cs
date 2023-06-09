﻿using Envelope.ServiceBus.Messages;
using Envelope.ServiceBus.Writers;
using Envelope.Trace;
using Envelope.Transactions;

namespace Envelope.EnterpriseServiceBus.Writers.Internal;

internal class DefaultJobMessageWriter : IJobMessageWriter, IJobMessagePublisher
{
	public Task WriteActiveJobMessageAsync(
		IJobMessage message,
		ITraceInfo traceInfo,
		ITransactionController? transactionController,
		CancellationToken cancellationToken = default)
		=> Task.CompletedTask;

	public Task WriteCompleteAsync(
		IJobMessage message,
		ITraceInfo traceInfo,
		ITransactionController? transactionController,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null,
		CancellationToken cancellationToken = default)
		=> Task.CompletedTask;

	public Task WriteSetErrorRetryAsync(
		IJobMessage message,
		ITraceInfo traceInfo,
		DateTime? delayedToUtc,
		TimeSpan? delay,
		int maxRetryCount,
		ITransactionController? transactionController,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null,
		CancellationToken cancellationToken = default)
		=> Task.CompletedTask;

	public Task WriteSuspendAsync(
		IJobMessage message,
		ITraceInfo traceInfo,
		ITransactionController? transactionController,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null,
		CancellationToken cancellationToken = default)
		=> Task.CompletedTask;

	public Task WriteResumeAsync(
		IJobMessage message,
		ITraceInfo traceInfo,
		ITransactionController? transactionController,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null,
		CancellationToken cancellationToken = default)
		=> Task.CompletedTask;

	public Task WriteDeleteAsync(
		IJobMessage message,
		ITraceInfo traceInfo,
		ITransactionController? transactionController,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null,
		CancellationToken cancellationToken = default)
		=> Task.CompletedTask;

	public Task WriteArchiveJobMessageAsync(
		IJobMessage message,
		ITraceInfo traceInfo,
		ITransactionController? transactionController,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null,
		bool removeFromActiveMessages = true,
		CancellationToken cancellationToken = default)
		=> Task.CompletedTask;

	public Task PublishJobMessageAsync(
		ITraceInfo traceInfo,
		int jobMessageTypeId,
		ITransactionController? transactionController,
		Guid? taskCorrelationId = null,
		int priority = 100,
		DateTime? timeToLive = null,
		string? entityName = null,
		Guid? entityId = null,
		Dictionary<string, object?>? properties = null,
		string? detail = null,
		bool? isDetailJson = null,
		CancellationToken cancellationToken = default)
		=> Task.CompletedTask;
}
