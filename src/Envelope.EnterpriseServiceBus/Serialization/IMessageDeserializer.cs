using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.Serialization;

public interface IMessageDeserializer
{
	string ContentType { get; }

	TMessage? Deserialize<TMessage>(IMessageMetadata messageMetadata, IMessageBody? messageBody)
		where TMessage : class, IMessage;
}
