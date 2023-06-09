﻿using Envelope.EnterpriseServiceBus.Queues;
using Envelope.Services;
using Envelope.Trace;
using System.Runtime.CompilerServices;

namespace Envelope.EnterpriseServiceBus.MessageHandlers;

public interface IMessageHandlerResultFactory
{
	MessageHandlerResult FromResult(
		IResult result,
		ITraceInfo traceInfo,
		TimeSpan? errorRetryInterval = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0);

	MessageHandlerResult Completed();

	MessageHandlerResult Error(IResult errorResult, TimeSpan? retryInterval = null);

	MessageHandlerResult Suspended(IResult errorResult);

	MessageHandlerResult Deferred(TimeSpan retryInterval);

	MessageHandlerResult DeliveredInternal(IMessageQueue? onMessageQueue);

	MessageHandlerResult AbortedInternal(IResult errorResult);



	MessageHandlerResult<TResponse> FromResult<TResponse>(
		IResult<TResponse> result,
		ITraceInfo traceInfo,
		TimeSpan? errorRetryInterval = null,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0);

	MessageHandlerResult<TResponse> Completed<TResponse>(TResponse result);

	MessageHandlerResult<TResponse> Error<TResponse>(IResult errorResult, TimeSpan? retryInterval = null);

	MessageHandlerResult<TResponse> Suspended<TResponse>(IResult errorResult);

	MessageHandlerResult<TResponse> Deferred<TResponse>(TimeSpan retryInterval);

	MessageHandlerResult<TResponse> DeliveredInternal<TResponse>();

	MessageHandlerResult<TResponse> AbortedInternal<TResponse>(IResult errorResult);
}
