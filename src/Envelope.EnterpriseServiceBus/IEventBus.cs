﻿using Envelope.EnterpriseServiceBus.MessageHandlers;
using Envelope.EnterpriseServiceBus.Messages.Options;
using Envelope.ServiceBus.Messages;
using Envelope.Services;
using Envelope.Trace;
using System.Runtime.CompilerServices;

namespace Envelope.EnterpriseServiceBus;

public interface IEventBus : IEventPublisher
{
	/// <summary>
	/// Publishes an event
	/// </summary>
	/// <param name="event"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="memberName">Allows you to obtain the method or property name of the caller to the method.</param>
	/// <param name="sourceFilePath">Allows you to obtain the full path of the source file that contains the caller. This is the file path at the time of compile.</param>
	/// <param name="sourceLineNumber">Allows you to obtain the line number in the source file at which the method is called.</param>
	/// <returns>Created event ID or warning if no <see cref="MessageHandlerContext"/> was created</returns>
	Task<IResult<Guid>> PublishAsync(
		IEvent @event,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0);

	/// <summary>
	/// Publishes an event
	/// </summary>
	/// <param name="event"></param>
	/// <param name="optionsBuilder">Configure the message sending options</param>
	/// <param name="cancellationToken"></param>
	/// <param name="memberName">Allows you to obtain the method or property name of the caller to the method.</param>
	/// <param name="sourceFilePath">Allows you to obtain the full path of the source file that contains the caller. This is the file path at the time of compile.</param>
	/// <param name="sourceLineNumber">Allows you to obtain the line number in the source file at which the method is called.</param>
	/// <returns>Created event ID or warning if no <see cref="MessageHandlerContext"/> was created</returns>
	Task<IResult<Guid>> PublishAsync(
		IEvent @event,
		Action<MessageOptionsBuilder> optionsBuilder,
		CancellationToken cancellationToken = default,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0);

	/// <summary>
	/// Publishes an event
	/// </summary>
	/// <param name="event"></param>
	/// <param name="traceInfo"></param>
	/// <param name="cancellationToken"></param>
	/// <returns>Created event ID or warning if no <see cref="MessageHandlerContext"/> was created</returns>
	Task<IResult<Guid>> PublishAsync(
		IEvent @event,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Publishes an event
	/// </summary>
	/// <param name="event"></param>
	/// <param name="optionsBuilder">Configure the message sending options</param>
	/// <param name="traceInfo"></param>
	/// <param name="cancellationToken"></param>
	/// <returns>Created event ID or warning if no <see cref="MessageHandlerContext"/> was created</returns>
	Task<IResult<Guid>> PublishAsync(
		IEvent @event,
		Action<MessageOptionsBuilder>? optionsBuilder,
		ITraceInfo traceInfo,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Publishes an event
	/// </summary>
	/// <param name="event"></param>
	/// <param name="memberName">Allows you to obtain the method or property name of the caller to the method.</param>
	/// <param name="sourceFilePath">Allows you to obtain the full path of the source file that contains the caller. This is the file path at the time of compile.</param>
	/// <param name="sourceLineNumber">Allows you to obtain the line number in the source file at which the method is called.</param>
	/// <returns>Created event ID or warning if no <see cref="MessageHandlerContext"/> was created</returns>
	IResult<Guid> Publish(
		IEvent @event,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0);

	/// <summary>
	/// Publishes an event
	/// </summary>
	/// <param name="event"></param>
	/// <param name="optionsBuilder">Configure the message sending options</param>
	/// <param name="memberName">Allows you to obtain the method or property name of the caller to the method.</param>
	/// <param name="sourceFilePath">Allows you to obtain the full path of the source file that contains the caller. This is the file path at the time of compile.</param>
	/// <param name="sourceLineNumber">Allows you to obtain the line number in the source file at which the method is called.</param>
	/// <returns>Created event ID or warning if no <see cref="MessageHandlerContext"/> was created</returns>
	IResult<Guid> Publish(
		IEvent @event,
		Action<MessageOptionsBuilder> optionsBuilder,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0);

	/// <summary>
	/// Publishes an event
	/// </summary>
	/// <param name="event"></param>
	/// <param name="traceInfo"></param>
	/// <returns>Created event ID or warning if no <see cref="MessageHandlerContext"/> was created</returns>
	IResult<Guid> Publish(
		IEvent @event,
		ITraceInfo traceInfo);

	/// <summary>
	/// Publishes an event
	/// </summary>
	/// <param name="event"></param>
	/// <param name="optionsBuilder">Configure the message sending options</param>
	/// <param name="traceInfo"></param>
	/// <returns>Created event ID or warning if no <see cref="MessageHandlerContext"/> was created</returns>
	IResult<Guid> Publish(
		IEvent @event,
		Action<MessageOptionsBuilder>? optionsBuilder,
		ITraceInfo traceInfo);
}
