﻿using Envelope.Logging;
using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.ServiceBus.Hosts;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.Trace;
using Envelope.Transactions;

namespace Envelope.EnterpriseServiceBus.MessageHandlers;

public interface IMessageHandlerContext : IMessageMetadata
{
	IServiceBusOptions? ServiceBusOptions { get; }

	bool ThrowNoHandlerException { get; }

	/// <summary>
	/// Current transaction context
	/// </summary>
	ITransactionController TransactionController { get; }

	/// <summary>
	/// MessageBus's service provider
	/// </summary>
	IServiceProvider? ServiceProvider { get; }

	IMessageHandlerResultFactory MessageHandlerResultFactory { get; }

	/// <summary>
	/// ServiceBus Host
	/// </summary>
	IHostInfo HostInfo { get; }

	void Initialize(MessageHandlerContextOptions options, MessageStatus messageStatus, DateTime? delayedToUtc);

	ILogMessage? LogTrace(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null);

	ILogMessage? LogDebug(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null);

	ILogMessage? LogInformation(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		bool force = false,
		ITransactionCoordinator? transactionCoordinator = null);

	ILogMessage? LogWarning(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		bool force = false,
		ITransactionCoordinator? transactionCoordinator = null);

	IErrorMessage? LogError(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<ErrorMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null);

	IErrorMessage? LogCritical(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<ErrorMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null);

	Task<ILogMessage?> LogTraceAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default);

	Task<ILogMessage?> LogDebugAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default);

	Task<ILogMessage?> LogInformationAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		bool force = false,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default);

	Task<ILogMessage?> LogWarningAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		bool force = false,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default);

	Task<IErrorMessage?> LogErrorAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<ErrorMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default);

	Task<IErrorMessage?> LogCriticalAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<ErrorMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default);
}
