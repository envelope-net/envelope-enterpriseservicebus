using Envelope.EnterpriseServiceBus.Serialization;
using Envelope.EnterpriseServiceBus.Transformation;

namespace Envelope.EnterpriseServiceBus.Messages.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IMessageBodyProviderConfiguration
{
	IMessageSerializer MessageSerializer { get; set; }

	IMessageDeserializer MessageDeserializer { get; set; }

	ICompressionProvider? CompressionProvider { get; set; }

	IEncryptionProvider? EncryptionProvider { get; set; }
}
