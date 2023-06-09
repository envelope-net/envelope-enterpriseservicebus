﻿using Envelope.EnterpriseServiceBus.MessageHandlers.Interceptors;
using Envelope.Services;
using Envelope.Trace;
using Microsoft.Extensions.DependencyInjection;

namespace Envelope.EnterpriseServiceBus.MessageHandlers.Processors;

internal abstract class VoidMessageHandlerProcessor : MessageHandlerProcessorBase
{
	public abstract IResult Handle(
		Envelope.ServiceBus.Messages.IRequestMessage message,
		IMessageHandlerContext handlerContext,
		IServiceProvider serviceProvider,
		string? unhandledExceptionDetail);

	public abstract void OnError(
		ITraceInfo traceInfo,
		Exception? exception,
		IResult? errorResult,
		string? detail,
		Envelope.ServiceBus.Messages.IRequestMessage? message,
		IMessageHandlerContext? handlerContext,
		IServiceProvider serviceProvider);
}

internal class VoidMessageHandlerProcessor<TRequestMessage, TContext> : VoidMessageHandlerProcessor
	where TRequestMessage : Envelope.ServiceBus.Messages.IRequestMessage
	where TContext : IMessageHandlerContext
{
	protected override IMessageHandler CreateHandler(IServiceProvider serviceProvider)
	{
		var handler = serviceProvider.GetService<IMessageHandler<TRequestMessage, TContext>>();
		if (handler == null)
			throw new InvalidOperationException($"Could not resolve handler for {typeof(IMessageHandler<TRequestMessage, TContext>).FullName}");

		return handler;
	}

	public override IResult Handle(
		Envelope.ServiceBus.Messages.IRequestMessage message,
		IMessageHandlerContext handlerContext,
		IServiceProvider serviceProvider,
		string? unhandledExceptionDetail)
		=> Handle((TRequestMessage)message, (TContext)handlerContext, serviceProvider, unhandledExceptionDetail);

	public IResult Handle(
		TRequestMessage message,
		TContext handlerContext,
		IServiceProvider serviceProvider,
		string? unhandledExceptionDetail)
	{
		IMessageHandler<TRequestMessage, TContext>? handler = null;

		try
		{
			handler = (IMessageHandler<TRequestMessage, TContext>)CreateHandler(serviceProvider);

			IResult result;
			var interceptorType = handler.InterceptorType;
			if (interceptorType == null)
			{
				result = handler.Handle(message, handlerContext);
			}
			else
			{
				var interceptor = (IMessageHandlerInterceptor<TRequestMessage, TContext>?)serviceProvider.GetService(interceptorType);
				if (interceptor == null)
					throw new InvalidOperationException($"Could not resolve interceptor for {typeof(IMessageHandlerInterceptor<TRequestMessage, TContext>).FullName}");

				result = interceptor.InterceptHandle(message, handlerContext, handler.Handle);
			}

			if (result.HasError)
			{
				var traceInfo = TraceInfo.Create(handlerContext.TraceInfo);
				try
				{
					handler.OnError(traceInfo, null, result, unhandledExceptionDetail, message, handlerContext);
				}
				catch (Exception onErrorEx)
				{
					try
					{
						handlerContext.LogCritical(traceInfo, null, x => x.ExceptionInfo(onErrorEx), "OnError: Send<Envelope.ServiceBus.Messages.IRequestMessage> error", null);
					}
					catch { }
				}
			}

			return result;
		}
		catch (Exception exHandler)
		{
			var traceInfo = TraceInfo.Create(handlerContext.TraceInfo);
			try
			{
				handlerContext.LogError(traceInfo, null, x => x.ExceptionInfo(exHandler), "Send<Envelope.ServiceBus.Messages.IRequestMessage> error", null);
			}
			catch { }

			if (handler != null)
			{
				try
				{
					handler.OnError(traceInfo, exHandler, null, unhandledExceptionDetail, message, handlerContext);
				}
				catch (Exception onErrorEx)
				{
					try
					{
						handlerContext.LogCritical(traceInfo, null, x => x.ExceptionInfo(onErrorEx), "OnError: Send<Envelope.ServiceBus.Messages.IRequestMessage> error", null);
					}
					catch { }
				}
			}

			throw;
		}
	}

	public override void OnError(
		ITraceInfo traceInfo,
		Exception? exception,
		IResult? errorResult,
		string? detail,
		Envelope.ServiceBus.Messages.IRequestMessage? message,
		IMessageHandlerContext? handlerContext,
		IServiceProvider serviceProvider)
		=> OnError(traceInfo, exception, errorResult, detail, (TRequestMessage?)message, (TContext?)handlerContext, serviceProvider);

	public void OnError(
		ITraceInfo traceInfo,
		Exception? exception,
		IResult? errorResult,
		string? detail,
		TRequestMessage? message,
		TContext? handlerContext,
		IServiceProvider serviceProvider)
	{
		try
		{
			var handler = (IMessageHandler<TRequestMessage, TContext>)CreateHandler(serviceProvider);
			handler.OnError(traceInfo, exception, errorResult, detail, message, handlerContext);
		}
		catch (Exception onErrorEx)
		{
			try
			{
				if (handlerContext != null)
					handlerContext.LogCritical(traceInfo, null, x => x.ExceptionInfo(onErrorEx), "OnErrorAsync: SendAsync<Envelope.ServiceBus.Messages.IRequestMessage> error", null);
			}
			catch { }
		}
	}
}
