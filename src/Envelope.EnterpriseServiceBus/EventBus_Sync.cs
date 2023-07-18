using Envelope.EnterpriseServiceBus.Internals;
using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.MessageHandlers.Processors;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.Messages.Options;
using Envelope.ServiceBus.Messages;
using Envelope.Services;
using Envelope.Services.Transactions;
using Envelope.Trace;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Envelope.EnterpriseServiceBus;

public partial class EventBus : IEventBus
{
	/// <inheritdoc />
	public IResult<Guid> Publish(
		IEvent @event,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> Publish(@event, null!, memberName, sourceFilePath, sourceLineNumber);

	/// <inheritdoc />
	public IResult<Guid> Publish(
		IEvent @event,
		Action<MessageOptionsBuilder> optionsBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
		=> Publish(
			@event,
			optionsBuilder,
			TraceInfo.Create(
				ServiceProvider.GetRequiredService<IApplicationContext>().TraceInfo,
				null, //EventBusOptions.HostInfo.HostName,
				null,
				memberName,
				sourceFilePath,
				sourceLineNumber));

	/// <inheritdoc />
	public IResult<Guid> Publish(
		IEvent @event,
		ITraceInfo traceInfo)
		=> Publish(@event, null, traceInfo);

	/// <inheritdoc />
	public IResult<Guid> Publish(
		IEvent @event,
		Action<MessageOptionsBuilder>? optionsBuilder,
		ITraceInfo traceInfo)
	{
		if (@event == null)
		{
			var result = new ResultBuilder<Guid>();
			return result.WithArgumentNullException(traceInfo, nameof(@event));
		}

		var builder = MessageOptionsBuilder.GetDefaultBuilder(@event.GetType());
		optionsBuilder?.Invoke(builder);
		var options = builder.Build(true);

		var isLocalTransactionCoordinator = false;
		if (options.TransactionController == null)
		{
			options.TransactionController = CreateTransactionController();
			isLocalTransactionCoordinator = true;
		}

		return PublishInternal(@event, options, isLocalTransactionCoordinator, traceInfo);
	}

	protected IResult<Guid> PublishInternal(
		IEvent @event,
		IMessageOptions options,
		bool isLocalTransactionCoordinator,
		ITraceInfo traceInfo)
	{
		var result = new ResultBuilder<Guid>();

		if (@event == null)
			return result.WithArgumentNullException(traceInfo, nameof(@event));
		if (options == null)
			return result.WithArgumentNullException(traceInfo, nameof(options));
		if (traceInfo == null)
			return result.WithArgumentNullException(
				TraceInfo.Create(
					ServiceProvider.GetRequiredService<IApplicationContext>().TraceInfo
					//EventBusOptions.HostInfo.HostName
					),
				nameof(traceInfo));

		traceInfo = TraceInfo.Create(traceInfo);

		var transactionController = options.TransactionController;
		EventHandlerProcessor? handlerProcessor = null;
		IMessageHandlerContext? handlerContext = null;

		return ServiceTransactionInterceptor.ExecuteAction(
			false,
			traceInfo,
			transactionController,
			(traceInfo, transactionController, unhandledExceptionDetail) =>
			{
				var eventType = @event.GetType();

				var savedEventResult = SaveEvent(@event, options, traceInfo);
				if (result.MergeHasError(savedEventResult))
					return result.Build();

				var savedEvent = savedEventResult.Data;

				if (savedEvent == null)
					return result.WithInvalidOperationException(traceInfo, $"{nameof(savedEvent)} == null | {nameof(eventType)} = {eventType.FullName}");
				if (savedEvent.Message == null)
					return result.WithInvalidOperationException(traceInfo, $"{nameof(savedEvent)}.{nameof(savedEvent.Message)} == null | {nameof(eventType)} = {eventType.FullName}");

				handlerContext = EventHandlerRegistry.CreateEventHandlerContext(eventType, ServiceProvider);

				var throwNoHandlerException = options.ThrowNoHandlerException ?? false;

				if (handlerContext == null)
				{
					if (throwNoHandlerException)
					{
						return result.WithInvalidOperationException(traceInfo, $"{nameof(handlerContext)} == null| {nameof(eventType)} = {eventType.FullName}");
					}
					else
					{
						return result.WithWarning(traceInfo, $"{nameof(handlerContext)} == null| {nameof(eventType)} = {eventType.FullName}");
					}
				}

				handlerContext.Initialize(new MessageHandlerContextOptions
				{
					MessageHandlerResultFactory = EventBusOptions.MessageHandlerResultFactory,
					TransactionController = transactionController,
					ServiceProvider = ServiceProvider,
					TraceInfo = traceInfo,
					HostInfo = EventBusOptions.HostInfo,
					HandlerLogger = EventBusOptions.HandlerLogger,
					MessageId = savedEvent.MessageId,
					DisabledMessagePersistence = options.DisabledMessagePersistence,
					ThrowNoHandlerException = throwNoHandlerException,
					PublisherId = PublisherHelper.GetPublisherIdentifier(EventBusOptions.HostInfo, traceInfo),
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

				handlerProcessor = (EventHandlerProcessor)_asyncVoidEventHandlerProcessors.GetOrAdd(
					eventType,
					eventType =>
					{
						var processor = Activator.CreateInstance(typeof(EventHandlerProcessor<,>).MakeGenericType(eventType, handlerContext.GetType())) as EventHandlerProcessorBase;

						if (processor == null)
							result.WithInvalidOperationException(traceInfo, $"Could not create handlerProcessor type for {eventType}");

						return processor!;
					});

				if (result.HasError())
					return result.Build();

				if (handlerProcessor == null)
					return result.WithInvalidOperationException(traceInfo, $"Could not create handlerProcessor type for {eventType}");

				var handlerResult = handlerProcessor.Handle(savedEvent.Message, handlerContext, ServiceProvider, unhandledExceptionDetail);
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

				return result.WithData(savedEvent.MessageId).Build();
			},
			$"{nameof(Publish)}<{@event?.GetType().FullName}>",
			(traceInfo, exception, detail) =>
			{
				var errorMessage =
					EventBusOptions.HostLogger.LogError(
						traceInfo,
						EventBusOptions.HostInfo,
						x => x.ExceptionInfo(exception).Detail(detail),
						detail,
						null);

				if (handlerProcessor != null)
				{
					try
					{
						handlerProcessor.OnError(traceInfo, exception, null, detail, @event, handlerContext, ServiceProvider);
					}
					catch { }
				}

				return errorMessage;
			},
			null,
			isLocalTransactionCoordinator);
	}

	protected virtual IResult<ISavedMessage<TEvent>> SaveEvent<TEvent>(
		TEvent @event,
		IMessageOptions options,
		ITraceInfo traceInfo)
		where TEvent : class, IEvent
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder<ISavedMessage<TEvent>>();

		var utcNow = DateTime.UtcNow;
		var metadata = new MessageMetadata<TEvent>
		{
			MessageId = Guid.NewGuid(),
			Message = @event,
			ParentMessageId = null,
			PublishingTimeUtc = utcNow,
			PublisherId = "--EventBus--",
			TraceInfo = traceInfo,
			Timeout = options.Timeout,
			RetryCount = 0,
			ErrorHandling = options.ErrorHandling,
			IdSession = options.IdSession,
			ContentType = options.ContentType,
			ContentEncoding = options.ContentEncoding,
			IsCompressedContent = options.IsCompressContent,
			IsEncryptedContent = options.IsEncryptContent,
			ContainsContent = @event != null,
			Priority = options.Priority,
			Headers = options.Headers?.GetAll(),
			DisabledMessagePersistence = options.DisabledMessagePersistence,
			MessageStatus = MessageStatus.Created,
			DelayedToUtc = null
		};

		if (EventBusOptions.EventBodyProvider != null
			&& EventBusOptions.EventBodyProvider.AllowMessagePersistence(options.DisabledMessagePersistence, metadata))
		{
			var saveResult = EventBusOptions.EventBodyProvider.SaveToStorage(new List<IMessageMetadata> { metadata }, @event, traceInfo, options.TransactionController);
			if (result.MergeHasError(saveResult))
				return result.Build();
		}

		return result.WithData(metadata).Build();
	}
}
