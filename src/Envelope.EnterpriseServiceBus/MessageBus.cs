using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Configuration.Internal;
using Envelope.EnterpriseServiceBus.Internals;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Processors;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.Messages.Options;
using Envelope.Exceptions;
using Envelope.ServiceBus.Hosts;
using Envelope.ServiceBus.Messages;
using Envelope.Services;
using Envelope.Services.Transactions;
using Envelope.Trace;
using Envelope.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Envelope.EnterpriseServiceBus;

public partial class MessageBus : IMessageBus
{
	protected IServiceProvider ServiceProvider { get; }
	protected IMessageBusOptions MessageBusOptions { get; }
	protected IMessageHandlerRegistry MessageHandlerRegistry { get; }

	private static readonly ConcurrentDictionary<Type, MessageHandlerProcessorBase> _asyncMessageHandlerProcessors = new();
	private static readonly ConcurrentDictionary<Type, MessageHandlerProcessorBase> _asyncVoidMessageHandlerProcessors = new();

	public MessageBus(IServiceProvider serviceProvider, IMessageBusConfiguration configuration, IMessageHandlerRegistry messageHandlerRegistry)
	{
		ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		MessageHandlerRegistry = messageHandlerRegistry ?? throw new ArgumentNullException(nameof(messageHandlerRegistry));

		if (configuration == null)
			throw new ArgumentNullException(nameof(configuration));

		var error = configuration.Validate(nameof(MessageBusConfiguration));
		if (0 < error?.Count)
			throw new ConfigurationException(error);

		MessageBusOptions = new MessageBusOptions()
		{
			HostInfo = new HostInfo(configuration.MessageBusName),
			HostLogger = configuration.HostLogger(serviceProvider),
			HandlerLogger = configuration.HandlerLogger(serviceProvider),
			MessageHandlerResultFactory = configuration.MessageHandlerResultFactory(serviceProvider),
			MessageBodyProvider = configuration.MessageBodyProvider
		};

		error = MessageBusOptions.Validate(nameof(MessageBusOptions));
		if (0 < error?.Count)
			throw new ConfigurationException(error);
	}

	public Task<IResult> SendAsync(
		IRequestMessage message,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> SendAsync(message, null!, cancellationToken, memberName, sourceFilePath, sourceLineNumber);

	public Task<IResult> SendAsync(
		IRequestMessage message,
		Action<MessageOptionsBuilder> optionsBuilder,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> SendAsync(
			message,
			optionsBuilder,
			TraceInfo.Create(
				ServiceProvider.GetRequiredService<IApplicationContext>().TraceInfo,
				null, //MessageBusOptions.HostInfo.HostName,
				null,
				memberName,
				sourceFilePath,
				sourceLineNumber),
			cancellationToken);

	public Task<IResult> SendAsync(
		IRequestMessage message,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
		=> SendAsync(message, (Action<MessageOptionsBuilder>?)null, traceInfo, cancellationToken);

	public async Task<IResult> SendAsync(
		IRequestMessage message,
		Action<MessageOptionsBuilder>? optionsBuilder,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
	{
		var result = new ResultBuilder();

		if (message == null)
			return result.WithArgumentNullException(traceInfo, nameof(message));

		var builder = MessageOptionsBuilder.GetDefaultBuilder(message.GetType());
		optionsBuilder?.Invoke(builder);
		var options = builder.Build(true);

		var isLocalTransactionCoordinator = false;
		if (options.TransactionController == null)
		{
			options.TransactionController = CreateTransactionController();
			isLocalTransactionCoordinator = true;
		}

		var sendResult = await SendInternalAsync(message, options, isLocalTransactionCoordinator, traceInfo, cancellationToken);
		return sendResult;
	}

	public Task<IResult<Guid>> SendWithMessageIdAsync(
		IRequestMessage message,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> SendWithMessageIdAsync(message, null!, cancellationToken, memberName, sourceFilePath, sourceLineNumber);

	public Task<IResult<Guid>> SendWithMessageIdAsync(
		IRequestMessage message,
		Action<MessageOptionsBuilder> optionsBuilder,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> SendWithMessageIdAsync(
			message,
			optionsBuilder,
			TraceInfo.Create(
				ServiceProvider.GetRequiredService<IApplicationContext>().TraceInfo,
				null, //MessageBusOptions.HostInfo.HostName,
				null,
				memberName,
				sourceFilePath,
				sourceLineNumber),
			cancellationToken);

	public Task<IResult<Guid>> SendWithMessageIdAsync(
		IRequestMessage message,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
		=> SendWithMessageIdAsync(message, (Action<MessageOptionsBuilder>?)null, traceInfo, cancellationToken);

	public Task<IResult<Guid>> SendWithMessageIdAsync(
		IRequestMessage message,
		Action<MessageOptionsBuilder>? optionsBuilder,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
	{
		if (message == null)
		{
			var result = new ResultBuilder<Guid>();
			return Task.FromResult(result.WithArgumentNullException(traceInfo, nameof(message)));
		}

		var builder = MessageOptionsBuilder.GetDefaultBuilder(message.GetType());
		optionsBuilder?.Invoke(builder);
		var options = builder.Build(true);

		var isLocalTransactionCoordinator = false;
		if (options.TransactionController == null)
		{
			options.TransactionController = CreateTransactionController();
			isLocalTransactionCoordinator = true;
		}

		return SendInternalAsync(message, options, isLocalTransactionCoordinator, traceInfo, cancellationToken);
	}

	protected async Task<IResult<Guid>> SendInternalAsync(
		IRequestMessage message,
		IMessageOptions options,
		bool isLocalTransactionCoordinator,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
	{
		var result = new ResultBuilder<Guid>();

		if (message == null)
			return result.WithArgumentNullException(traceInfo, nameof(message));
		if (options == null)
			return result.WithArgumentNullException(traceInfo, nameof(options));
		if (traceInfo == null)
			return result.WithArgumentNullException(
				TraceInfo.Create(
					ServiceProvider.GetRequiredService<IApplicationContext>().TraceInfo
					//MessageBusOptions.HostInfo.HostName
					),
				nameof(traceInfo));

		traceInfo = TraceInfo.Create(traceInfo);

		var transactionController = options.TransactionController;
		AsyncVoidMessageHandlerProcessor? handlerProcessor = null;
		IMessageHandlerContext? handlerContext = null;

		return await ServiceTransactionInterceptor.ExecuteActionAsync(
			false,
			traceInfo,
			transactionController,
			async (traceInfo, transactionController, unhandledExceptionDetail, cancellationToken) =>
			{
				var requestMessageType = message.GetType();

				var savedMessageResult = await SaveRequestMessageAsync(message, options, traceInfo, cancellationToken).ConfigureAwait(false);
				if (result.MergeHasError(savedMessageResult))
					return result.Build();

				var savedMessage = savedMessageResult.Data;

				if (savedMessage == null)
					return result.WithInvalidOperationException(traceInfo, $"{nameof(savedMessage)} == null | {nameof(requestMessageType)} = {requestMessageType.FullName}");
				if (savedMessage.Message == null)
					return result.WithInvalidOperationException(traceInfo, $"{nameof(savedMessage)}.{nameof(savedMessage.Message)} == null | {nameof(requestMessageType)} = {requestMessageType.FullName}");

				handlerContext = MessageHandlerRegistry.CreateMessageHandlerContext(requestMessageType, ServiceProvider);

				if (handlerContext == null)
					return result.WithInvalidOperationException(traceInfo, $"{nameof(handlerContext)} == null| {nameof(requestMessageType)} = {requestMessageType.FullName}");

				handlerContext.Initialize(new MessageHandlerContextOptions
				{
					MessageHandlerResultFactory = MessageBusOptions.MessageHandlerResultFactory,
					TransactionController = transactionController,
					ServiceProvider = ServiceProvider,
					TraceInfo = traceInfo,
					HostInfo = MessageBusOptions.HostInfo,
					HandlerLogger = MessageBusOptions.HandlerLogger,
					MessageId = savedMessage.MessageId,
					DisabledMessagePersistence = options.DisabledMessagePersistence,
					ThrowNoHandlerException = true,
					PublisherId = PublisherHelper.GetPublisherIdentifier(MessageBusOptions.HostInfo, traceInfo),
					PublishingTimeUtc = DateTime.UtcNow,
					ParentMessageId = null,
					Timeout = options.Timeout,
					RetryCount = 0,
					ErrorHandling = options.ErrorHandling,
					IdSession = options.IdSession,
					ContentType = options.ContentType,
					ContentEncoding = options.ContentEncoding,
					IsCompressedContent = options.IsCompressContent,
					IsEncryptedContent = options.IsEncryptContent,
					ContainsContent = true,
					Priority = options.Priority,
					Headers = options.Headers?.GetAll()
				},
				MessageStatus.Created,
				null);

				handlerProcessor = (AsyncVoidMessageHandlerProcessor)_asyncVoidMessageHandlerProcessors.GetOrAdd(
					requestMessageType,
					requestMessageType =>
					{
						var processor = Activator.CreateInstance(typeof(AsyncVoidMessageHandlerProcessor<,>).MakeGenericType(requestMessageType, handlerContext.GetType())) as MessageHandlerProcessorBase;

						if (processor == null)
							result.WithInvalidOperationException(traceInfo, $"Could not create handlerProcessor type for {requestMessageType}");

						return processor!;
					});

				if (result.HasError())
					return result.Build();

				if (handlerProcessor == null)
					return result.WithInvalidOperationException(traceInfo, $"Could not create handlerProcessor type for {requestMessageType}");

				var handlerResult = await handlerProcessor.HandleAsync(savedMessage.Message, handlerContext, ServiceProvider, unhandledExceptionDetail, cancellationToken).ConfigureAwait(false);
				result.MergeAll(handlerResult);

				if (result.HasTransactionRollbackError())
				{
					transactionController.ScheduleRollback();
				}
				else
				{
					if (isLocalTransactionCoordinator)
						transactionController.ScheduleCommit();
				}

				return result.WithData(savedMessage.MessageId).Build();
			},
			$"{nameof(SendAsync)}<{message?.GetType().FullName}> return {typeof(IResult<Guid>).FullName}",
			async (traceInfo, exception, detail) =>
			{
				var errorMessage =
					await MessageBusOptions.HostLogger.LogErrorAsync(
						traceInfo,
						MessageBusOptions.HostInfo,
						x => x.ExceptionInfo(exception).Detail(detail),
						detail,
						null,
						cancellationToken: default).ConfigureAwait(false);

				if (handlerProcessor != null)
				{
					try
					{
						await handlerProcessor.OnErrorAsync(traceInfo, exception, null, detail, message, handlerContext, ServiceProvider, cancellationToken);
					}
					catch { }
				}

				return errorMessage;
			},
			null,
			isLocalTransactionCoordinator,
			cancellationToken).ConfigureAwait(false);
	}

	public Task<IResult<TResponse>> SendAsync<TResponse>(
		IRequestMessage<TResponse> message,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> SendAsync(message, null!, cancellationToken, memberName, sourceFilePath, sourceLineNumber);

	public Task<IResult<TResponse>> SendAsync<TResponse>(
		IRequestMessage<TResponse> message,
		Action<MessageOptionsBuilder> optionsBuilder,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> SendAsync(
			message,
			optionsBuilder,
			TraceInfo.Create(
				ServiceProvider.GetRequiredService<IApplicationContext>().TraceInfo,
				null, //MessageBusOptions.HostInfo.HostName,
				null,
				memberName,
				sourceFilePath,
				sourceLineNumber),
			cancellationToken);

	public Task<IResult<TResponse>> SendAsync<TResponse>(
		IRequestMessage<TResponse> message,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
		=> SendAsync(message, null, traceInfo, cancellationToken);

	public async Task<IResult<TResponse>> SendAsync<TResponse>(
		IRequestMessage<TResponse> message,
		Action<MessageOptionsBuilder>? optionsBuilder,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
	{
		var result = new ResultBuilder<TResponse>();

		if (message == null)
			return result.WithArgumentNullException(traceInfo, nameof(message));

		var builder = MessageOptionsBuilder.GetDefaultBuilder(message.GetType());
		optionsBuilder?.Invoke(builder);
		var options = builder.Build(true);

		var isLocalTransactionCoordinator = false;
		if (options.TransactionController == null)
		{
			options.TransactionController = CreateTransactionController();
			isLocalTransactionCoordinator = true;
		}

		var sendResult = await SendInternalAsync(message, options, isLocalTransactionCoordinator, traceInfo, cancellationToken).ConfigureAwait(false);
		result.MergeAll(sendResult);

		if (sendResult.Data != null)
			result.WithData(sendResult.Data.Response);

		return result.Build();
	}

	public Task<IResult<ISendResponse<TResponse>>> SendWithMessageIdAsync<TResponse>(
		IRequestMessage<TResponse> message,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> SendWithMessageIdAsync(message, null!, cancellationToken, memberName, sourceFilePath, sourceLineNumber);

	public Task<IResult<ISendResponse<TResponse>>> SendWithMessageIdAsync<TResponse>(
		IRequestMessage<TResponse> message,
		Action<MessageOptionsBuilder> optionsBuilder,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> SendWithMessageIdAsync(
			message,
			optionsBuilder,
			TraceInfo.Create(
				ServiceProvider.GetRequiredService<IApplicationContext>().TraceInfo,
				null, //MessageBusOptions.HostInfo.HostName,
				null,
				memberName,
				sourceFilePath,
				sourceLineNumber),
			cancellationToken);

	public Task<IResult<ISendResponse<TResponse>>> SendWithMessageIdAsync<TResponse>(
		IRequestMessage<TResponse> message,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
		=> SendWithMessageIdAsync(message, null, traceInfo, cancellationToken);

	public Task<IResult<ISendResponse<TResponse>>> SendWithMessageIdAsync<TResponse>(
		IRequestMessage<TResponse> message,
		Action<MessageOptionsBuilder>? optionsBuilder,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
	{
		if (message == null)
		{
			var result = new ResultBuilder<ISendResponse<TResponse>>();
			return Task.FromResult(result.WithArgumentNullException(traceInfo, nameof(message)));
		}

		var builder = MessageOptionsBuilder.GetDefaultBuilder(message.GetType());
		optionsBuilder?.Invoke(builder);
		var options = builder.Build(true);

		var isLocalTransactionCoordinator = false;
		if (options.TransactionController == null)
		{
			options.TransactionController = CreateTransactionController();
			isLocalTransactionCoordinator = true;
		}

		return SendInternalAsync(message, options, isLocalTransactionCoordinator, traceInfo, cancellationToken);
	}

	protected async Task<IResult<ISendResponse<TResponse>>> SendInternalAsync<TResponse>(
		IRequestMessage<TResponse> message,
		IMessageOptions options,
		bool isLocalTransactionCoordinator,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
	{
		var result = new ResultBuilder<ISendResponse<TResponse>>();

		if (message == null)
			return result.WithArgumentNullException(traceInfo, nameof(message));
		if (options == null)
			return result.WithArgumentNullException(traceInfo, nameof(options));
		if (traceInfo == null)
			return result.WithArgumentNullException(
				TraceInfo.Create(
					ServiceProvider.GetRequiredService<IApplicationContext>().TraceInfo
					//MessageBusOptions.HostInfo.HostName
					),
				nameof(traceInfo));

		traceInfo = TraceInfo.Create(traceInfo);

		var transactionController = options.TransactionController;
		AsyncMessageHandlerProcessor<TResponse>? handlerProcessor = null;
		IMessageHandlerContext? handlerContext = null;

		return await ServiceTransactionInterceptor.ExecuteActionAsync(
			false,
			traceInfo,
			transactionController,
			async (traceInfo, transactionController, unhandledExceptionDetail, cancellationToken) =>
			{
				var requestMessageType = message.GetType();

				var savedMessageResult = await SaveRequestMessageAsync<IRequestMessage<TResponse>, TResponse>(message, options, traceInfo, cancellationToken).ConfigureAwait(false);
				if (result.MergeHasError(savedMessageResult))
					return result.Build();

				var savedMessage = savedMessageResult.Data;

				if (savedMessage == null)
					return result.WithInvalidOperationException(traceInfo, $"{nameof(savedMessage)} == null | {nameof(requestMessageType)} = {requestMessageType.FullName}");
				if (savedMessage.Message == null)
					return result.WithInvalidOperationException(traceInfo, $"{nameof(savedMessage)}.{nameof(savedMessage.Message)} == null | {nameof(requestMessageType)} = {requestMessageType.FullName}");

				handlerContext = MessageHandlerRegistry.CreateMessageHandlerContext(requestMessageType, ServiceProvider);

				if (handlerContext == null)
					return result.WithInvalidOperationException(traceInfo, $"{nameof(handlerContext)} == null| {nameof(requestMessageType)} = {requestMessageType.FullName}");

				handlerContext.Initialize(new MessageHandlerContextOptions
				{
					MessageHandlerResultFactory = MessageBusOptions.MessageHandlerResultFactory,
					TransactionController = transactionController,
					ServiceProvider = ServiceProvider,
					TraceInfo = traceInfo,
					HostInfo = MessageBusOptions.HostInfo,
					HandlerLogger = MessageBusOptions.HandlerLogger,
					MessageId = savedMessage.MessageId,
					DisabledMessagePersistence = options.DisabledMessagePersistence,
					ThrowNoHandlerException = true,
					PublisherId = PublisherHelper.GetPublisherIdentifier(MessageBusOptions.HostInfo, traceInfo),
					PublishingTimeUtc = DateTime.UtcNow,
					ParentMessageId = null,
					Timeout = options.Timeout,
					RetryCount = 0,
					ErrorHandling = options.ErrorHandling,
					IdSession = options.IdSession,
					ContentType = options.ContentType,
					ContentEncoding = options.ContentEncoding,
					IsCompressedContent = options.IsCompressContent,
					IsEncryptedContent = options.IsEncryptContent,
					ContainsContent = true,
					Priority = options.Priority,
					Headers = options.Headers?.GetAll()
				},
				MessageStatus.Created,
				null);

				handlerProcessor = (AsyncMessageHandlerProcessor<TResponse>)_asyncVoidMessageHandlerProcessors.GetOrAdd(
					requestMessageType,
					requestMessageType =>
					{
						var processor = Activator.CreateInstance(typeof(AsyncMessageHandlerProcessor<,,>).MakeGenericType(requestMessageType, typeof(TResponse), handlerContext.GetType())) as MessageHandlerProcessorBase;

						if (processor == null)
							result.WithInvalidOperationException(traceInfo, $"Could not create handlerProcessor type for {requestMessageType}");

						return processor!;
					});

				if (result.HasError())
					return result.Build();

				if (handlerProcessor == null)
					return result.WithInvalidOperationException(traceInfo, $"Could not create handlerProcessor type for {requestMessageType}");

				var handlerResult = await handlerProcessor.HandleAsync(savedMessage.Message, handlerContext, ServiceProvider, traceInfo, SaveResponseMessageAsync, unhandledExceptionDetail, cancellationToken).ConfigureAwait(false);
				result.MergeAll(handlerResult);

				if (result.HasTransactionRollbackError())
				{
					transactionController.ScheduleRollback();
				}
				else
				{
					if (isLocalTransactionCoordinator)
						transactionController.ScheduleCommit();
				}

				return result.WithData(handlerResult.Data).Build();
			},
			$"{nameof(SendAsync)}<{message?.GetType().FullName}> return {typeof(IResult<ISendResponse<TResponse>>).FullName}",
			async (traceInfo, exception, detail) =>
			{
				var errorMessage =
					await MessageBusOptions.HostLogger.LogErrorAsync(
						traceInfo,
						MessageBusOptions.HostInfo,
						x => x.ExceptionInfo(exception).Detail(detail),
						detail,
						null,
						cancellationToken: default).ConfigureAwait(false);

				if (handlerProcessor != null)
				{
					try
					{
						await handlerProcessor.OnErrorAsync(traceInfo, exception, null, detail, message, handlerContext, ServiceProvider, cancellationToken);
					}
					catch { }
				}

				return errorMessage;
			},
			null,
			isLocalTransactionCoordinator,
			cancellationToken).ConfigureAwait(false);
	}

	protected virtual ITransactionController CreateTransactionController()
		=> ServiceProvider.GetRequiredService<ITransactionCoordinator>().TransactionController;

	protected virtual async Task<IResult<ISavedMessage<TMessage>>> SaveRequestMessageAsync<TMessage, TResponse>(
		TMessage requestMessage,
		IMessageOptions options,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
		where TMessage : class, IRequestMessage<TResponse>
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder<ISavedMessage<TMessage>>();

		var utcNow = DateTime.UtcNow;
		var metadata = new MessageMetadata<TMessage>
		{
			MessageId = Guid.NewGuid(),
			Message = requestMessage,
			ParentMessageId = null,
			PublishingTimeUtc = utcNow,
			PublisherId = "--MessageBus--",
			TraceInfo = traceInfo,
			Timeout = options.Timeout,
			RetryCount = 0,
			ErrorHandling = options.ErrorHandling,
			IdSession = options.IdSession,
			ContentType = options.ContentType,
			ContentEncoding = options.ContentEncoding,
			IsCompressedContent = options.IsCompressContent,
			IsEncryptedContent = options.IsEncryptContent,
			ContainsContent = requestMessage != null,
			Priority = options.Priority,
			Headers = options.Headers?.GetAll(),
			DisabledMessagePersistence = options.DisabledMessagePersistence,
			MessageStatus = MessageStatus.Created,
			DelayedToUtc = null
		};

		if (MessageBusOptions.MessageBodyProvider != null
			&& MessageBusOptions.MessageBodyProvider.AllowMessagePersistence(options.DisabledMessagePersistence, metadata))
		{
			var saveResult = await MessageBusOptions.MessageBodyProvider.SaveToStorageAsync(new List<IMessageMetadata> { metadata }, requestMessage, traceInfo, options.TransactionController, cancellationToken).ConfigureAwait(false);
			if (result.MergeHasError(saveResult))
				return result.Build();
		}

		return result.WithData(metadata).Build();
	}

	protected virtual async Task<IResult<ISavedMessage<TMessage>>> SaveRequestMessageAsync<TMessage>(
		TMessage requestMessage,
		IMessageOptions options,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
		where TMessage : class, IRequestMessage
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder<ISavedMessage<TMessage>>();

		var utcNow = DateTime.UtcNow;
		var metadata = new MessageMetadata<TMessage>
		{
			MessageId = Guid.NewGuid(),
			Message = requestMessage,
			ParentMessageId = null,
			PublishingTimeUtc = utcNow,
			PublisherId = "--MessageBus--",
			TraceInfo = traceInfo,
			Timeout = options.Timeout,
			RetryCount = 0,
			ErrorHandling = options.ErrorHandling,
			IdSession = options.IdSession,
			ContentType = options.ContentType,
			ContentEncoding = options.ContentEncoding,
			IsCompressedContent = options.IsCompressContent,
			IsEncryptedContent = options.IsEncryptContent,
			ContainsContent = requestMessage != null,
			Priority = options.Priority,
			Headers = options.Headers?.GetAll(),
			DisabledMessagePersistence = options.DisabledMessagePersistence,
			MessageStatus = MessageStatus.Created,
			DelayedToUtc = null
		};

		if (MessageBusOptions.MessageBodyProvider != null
			&& MessageBusOptions.MessageBodyProvider.AllowMessagePersistence(options.DisabledMessagePersistence, metadata))
		{
			var saveResult = await MessageBusOptions.MessageBodyProvider.SaveToStorageAsync(new List<IMessageMetadata> { metadata }, requestMessage, traceInfo, options.TransactionController, cancellationToken).ConfigureAwait(false);
			if (result.MergeHasError(saveResult))
				return result.Build();
		}

		return result.WithData(metadata).Build();
	}

	protected virtual async Task<IResult<Guid>> SaveResponseMessageAsync<TResponse>(
		TResponse responseMessage,
		IMessageHandlerContext handlerContext,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default)
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder<Guid>();

		if (MessageBusOptions.MessageBodyProvider != null)
		{
			var saveResult = await MessageBusOptions.MessageBodyProvider.SaveReplyToStorageAsync(handlerContext.MessageId, responseMessage, traceInfo, handlerContext.TransactionController, cancellationToken).ConfigureAwait(false);
			if (result.MergeHasError(saveResult))
				return result.Build();

			result.WithData(saveResult.Data);
		}

		return result.Build();
	}
}
