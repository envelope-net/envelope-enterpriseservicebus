using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.Logging;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.ServiceBus.Hosts;
using Envelope.Trace;
using Envelope.Transactions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Envelope.EnterpriseServiceBus.MessageHandlers;

public abstract class MessageHandlerContext : IMessageHandlerContext, IMessageMetadata, IMessageInfo
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public IServiceBusOptions? ServiceBusOptions { get; private set; }

	public bool ThrowNoHandlerException { get; private set; }

	public ITransactionController TransactionController { get; private set; }

	public IServiceProvider? ServiceProvider { get; private set; }
	public IMessageHandlerResultFactory MessageHandlerResultFactory { get; private set; }

	public ITraceInfo TraceInfo { get; private set; }

	public IHostInfo HostInfo { get; private set; }

	public string PublisherId { get; private set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	internal IHandlerLogger? HandlerLogger { get; set; }

	public Guid MessageId { get; private set; }

	public Guid? ParentMessageId { get; private set; }

	public bool Processed { get => false; set => throw new NotSupportedException($"Set {nameof(Processed)} is not supported in {nameof(MessageHandlerContext)}"); }

	public bool DisabledMessagePersistence { get; private set; }

	public DateTime PublishingTimeUtc { get; private set; }

	public TimeSpan? Timeout { get; private set; }

	public DateTime? TimeToLiveUtc => Timeout.HasValue
		? PublishingTimeUtc.Add(Timeout.Value)
		: null;

	public Guid? IdSession { get; private set; }

	public string? ContentType { get; private set; }

	public Encoding? ContentEncoding { get; private set; }

	public bool IsCompressedContent { get; private set; }

	public bool IsEncryptedContent { get; private set; }

	public bool ContainsContent { get; private set; }

	public bool HasSelfContent { get; set; }

	public int Priority { get; private set; }

	public IEnumerable<KeyValuePair<string, object>>? Headers { get; private set; }

	public MessageStatus MessageStatus { get; private set; }

	public int RetryCount { get; private set; }

	public IErrorHandlingController? ErrorHandling { get; private set; }

	public DateTime? DelayedToUtc { get; private set; }

	private bool initialized;
	private readonly object _initLock = new();
	void IMessageHandlerContext.Initialize(MessageHandlerContextOptions options, MessageStatus messageStatus, DateTime? delayedToUtc)
	{
		if (initialized)
			throw new InvalidOperationException($"{nameof(MessageHandlerContext)} already initialized");

		lock (_initLock)
		{
			if (initialized)
				throw new InvalidOperationException($"{nameof(MessageHandlerContext)} already initialized");

			MessageStatus = messageStatus;
			DelayedToUtc = delayedToUtc;
			initialized = true;
		}
	}

	internal void Update(bool processed, MessageStatus status, int retryCount, DateTime? delayedToUtc)
	{
		throw new NotImplementedException();

		//Processed = processed;
		//MessageStatus = status;
		//RetryCount = retryCount;
		//DelayedToUtc = delayedToUtc;
	}

	void IMessageMetadata.UpdateInternal(bool processed, MessageStatus status, int retryCount, DateTime? delayedToUtc)
		=> Update(processed, status, retryCount, delayedToUtc);

	//public MethodLogScope CreateScope(
	//	ILogger logger,
	//	IEnumerable<MethodParameter>? methodParameters = null,
	//	[CallerMemberName] string memberName = "",
	//	[CallerFilePath] string sourceFilePath = "",
	//	[CallerLineNumber] int sourceLineNumber = 0)
	//{
	//	if (logger == null)
	//		throw new ArgumentNullException(nameof(logger));

	//	var traceInfo =
	//		new TraceInfoBuilder(
	//			(ServiceProvider ?? ServiceBusOptions?.ServiceProvider)!,
	//			new TraceFrameBuilder(TraceInfo?.TraceFrame)
	//				.CallerMemberName(memberName)
	//				.CallerFilePath(sourceFilePath)
	//				.CallerLineNumber(sourceLineNumber == 0 ? (int?)null : sourceLineNumber)
	//				.MethodParameters(methodParameters)
	//				.Build(),
	//			TraceInfo)
	//			.Build();

	//	var disposable = logger.BeginScope(new Dictionary<string, Guid?>
	//	{
	//		[nameof(ILogMessage.TraceInfo.TraceFrame.MethodCallId)] = traceInfo.TraceFrame.MethodCallId,
	//		[nameof(ILogMessage.TraceInfo.CorrelationId)] = traceInfo.CorrelationId
	//	});

	//	var scope = new MethodLogScope(traceInfo, disposable);
	//	return scope;
	//}

	public ITraceInfo CreateTraceInfo(
		IEnumerable<MethodParameter>? methodParameters = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
	{
		var traceInfo =
			new TraceInfoBuilder(
				(ServiceProvider ?? ServiceBusOptions?.ServiceProvider)!,
				new TraceFrameBuilder(TraceInfo?.TraceFrame)
					.CallerMemberName(memberName)
					.CallerFilePath(sourceFilePath)
					.CallerLineNumber(sourceLineNumber == 0 ? (int?)null : sourceLineNumber)
					.MethodParameters(methodParameters)
					.Build(),
				TraceInfo)
				.Build();

		return traceInfo;
	}

	public virtual IErrorMessage? LogCritical(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<ErrorMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null)
		=> HandlerLogger?.LogCritical(traceInfo, messageMetadata, messageBuilder, detail, transactionCoordinator);

	public virtual Task<IErrorMessage?> LogCriticalAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<ErrorMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default)
	{
		if (HandlerLogger == null)
			return Task.FromResult((IErrorMessage?)null);
		else
			return HandlerLogger.LogCriticalAsync(traceInfo, messageMetadata, messageBuilder, detail, transactionCoordinator, cancellationToken)!;
	}

	public virtual ILogMessage? LogDebug(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null)
		=> HandlerLogger?.LogDebug(traceInfo, messageMetadata, messageBuilder, detail, transactionCoordinator);

	public virtual Task<ILogMessage?> LogDebugAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default)
		=> HandlerLogger == null
			? Task.FromResult((ILogMessage?)null)
			: HandlerLogger.LogDebugAsync(traceInfo, messageMetadata, messageBuilder, detail, transactionCoordinator, cancellationToken);

	public virtual IErrorMessage? LogError(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<ErrorMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null)
		=> HandlerLogger?.LogError(traceInfo, messageMetadata, messageBuilder, detail, transactionCoordinator);

	public virtual Task<IErrorMessage?> LogErrorAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<ErrorMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default)
	{
		if (HandlerLogger == null)
			return Task.FromResult((IErrorMessage?)null);
		else
			return HandlerLogger.LogErrorAsync(traceInfo, messageMetadata, messageBuilder, detail, transactionCoordinator, cancellationToken)!;
	}

	public virtual ILogMessage? LogInformation(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		bool force = false,
		ITransactionCoordinator? transactionCoordinator = null)
		=> HandlerLogger?.LogInformation(traceInfo, messageMetadata, messageBuilder, detail, force, transactionCoordinator);

	public virtual Task<ILogMessage?> LogInformationAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		bool force = false,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default)
		=> HandlerLogger == null
			? Task.FromResult((ILogMessage?)null)
			: HandlerLogger.LogInformationAsync(traceInfo, messageMetadata, messageBuilder, detail, force, transactionCoordinator, cancellationToken);

	public virtual ILogMessage? LogTrace(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null)
		=> HandlerLogger?.LogTrace(traceInfo, messageMetadata, messageBuilder, detail, transactionCoordinator);

	public virtual Task<ILogMessage?> LogTraceAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default)
		=> HandlerLogger == null
			? Task.FromResult((ILogMessage?)null)
			: HandlerLogger.LogTraceAsync(traceInfo, messageMetadata, messageBuilder, detail, transactionCoordinator, cancellationToken);

	public virtual ILogMessage? LogWarning(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		bool force = false,
		ITransactionCoordinator? transactionCoordinator = null)
		=> HandlerLogger?.LogWarning(traceInfo, messageMetadata, messageBuilder, detail, force, transactionCoordinator);

	public virtual Task<ILogMessage?> LogWarningAsync(
		ITraceInfo traceInfo,
		IMessageMetadata? messageMetadata,
		Action<LogMessageBuilder> messageBuilder,
		string? detail = null,
		bool force = false,
		ITransactionCoordinator? transactionCoordinator = null,
		CancellationToken cancellationToken = default)
		=> HandlerLogger == null
			? Task.FromResult((ILogMessage?)null)
			: HandlerLogger.LogWarningAsync(traceInfo, messageMetadata, messageBuilder, detail, force, transactionCoordinator, cancellationToken);
}

