﻿using Envelope.EnterpriseServiceBus.Messages;
using Envelope.ServiceBus.Messages;
using System.Diagnostics.CodeAnalysis;

namespace Envelope.EnterpriseServiceBus.Serialization;

public interface IMessageSerializer
{
	string ContentType { get; }

	[return: NotNullIfNotNull("message")]
	IMessageBody? Serialize<TMessage>(IMessageMetadata messageMetadata, TMessage? message)
		where TMessage : class, IMessage;
}
