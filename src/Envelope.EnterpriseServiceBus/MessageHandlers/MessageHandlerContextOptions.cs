using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.ServiceBus.Hosts;
using Envelope.Trace;
using Envelope.Transactions;
using System.Text;

namespace Envelope.EnterpriseServiceBus.MessageHandlers;

public class MessageHandlerContextOptions
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public IServiceBusOptions? ServiceBusOptions { get; set; }

	public bool ThrowNoHandlerException { get; set; }

	public ITransactionController TransactionController { get; set; }

	public IServiceProvider? ServiceProvider { get; set; }
	public IMessageHandlerResultFactory MessageHandlerResultFactory { get; set; }

	public ITraceInfo TraceInfo { get; set; }

	public IHostInfo HostInfo { get; set; }

	public string PublisherId { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public IHandlerLogger? HandlerLogger { get; set; }

	public Guid MessageId { get; set; }

	public Guid? ParentMessageId { get; set; }

	public bool Processed { get => false; set => throw new NotSupportedException($"Set {nameof(Processed)} is not supported in {nameof(MessageHandlerContext)}"); }

	public bool DisabledMessagePersistence { get; set; }

	public DateTime PublishingTimeUtc { get; set; }

	public TimeSpan? Timeout { get; set; }

	public DateTime? TimeToLiveUtc => Timeout.HasValue
		? PublishingTimeUtc.Add(Timeout.Value)
		: null;

	public Guid? IdSession { get; set; }

	public string? ContentType { get; set; }

	public Encoding? ContentEncoding { get; set; }

	public bool IsCompressedContent { get; set; }

	public bool IsEncryptedContent { get; set; }

	public bool ContainsContent { get; set; }

	public bool HasSelfContent { get; set; }

	public int Priority { get; set; }

	public IEnumerable<KeyValuePair<string, object>>? Headers { get; set; }

	public MessageStatus MessageStatus { get; private set; }

	public int RetryCount { get; set; }

	public IErrorHandlingController? ErrorHandling { get; set; }

	public DateTime? DelayedToUtc { get; private set; }
}

