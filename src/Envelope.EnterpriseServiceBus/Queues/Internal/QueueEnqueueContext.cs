﻿using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.Trace;
using System.Text;

namespace Envelope.EnterpriseServiceBus.Queues.Internal;

internal class QueueEnqueueContext : IQueueEnqueueContext
{
	public Guid MessageId { get; set; }

	public Guid? ParentMessageId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public string SourceExchangeName { get; set; }

	public string PublisherId { get; set; }

	public ITraceInfo TraceInfo { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public bool DisabledMessagePersistence { get; set; }

	public Guid? IdSession { get; set; }

	public string? ContentType { get; set; }

	public Encoding? ContentEncoding { get; set; }

	public string? RoutingKey { get; set; }

	public IErrorHandlingController? ErrorHandling { get; set; }

	public IMessageHeaders? Headers { get; set; }

	public bool IsAsynchronousInvocation { get; set; }

	public TimeSpan? Timeout { get; set; }

	public bool IsCompressedContent { get; set; }

	public bool IsEncryptedContent { get; set; }

	public int Priority { get; set; }

	public bool DisableFaultQueue { get; set; }
	
	public IMessageQueue? OnMessageQueueInternal { get; set; }
}
