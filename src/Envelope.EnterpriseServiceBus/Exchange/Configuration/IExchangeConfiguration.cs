using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Exchange.Routing;
using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus.ErrorHandling;
using Envelope.ServiceBus.Messages;
using Envelope.Validation;

namespace Envelope.EnterpriseServiceBus.Exchange.Configuration;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IExchangeConfiguration<TMessage> : IValidable
		where TMessage : class, IMessage
{
	IServiceBusOptions ServiceBusOptions { get; }

	/// <summary>
	/// Unique queue name on the host
	/// </summary>
	string ExchangeName { get; set; }

	QueueType QueueType { get; set; }

	/// <summary>
	/// Queue fetching start delay timeout
	/// </summary>
	TimeSpan? StartDelay { get; set; }

	/// <summary>
	/// Fetch messages interval
	/// </summary>
	TimeSpan FetchInterval { get; set; }

	/// <summary>
	/// Queue max size
	/// </summary>
	int? MaxSize { get; set; }

	Func<IServiceProvider, IExchangeMessageFactory<TMessage>> ExchangeMessageFactory { get; set; }

	Func<IServiceProvider, IMessageBrokerHandler<TMessage>> MessageBrokerHandler { get; set; }

	Func<IServiceProvider, int?, IQueue<IExchangeMessage<TMessage>>> FIFOQueue { get; set; }

	Func<IServiceProvider, int?, IQueue<IExchangeMessage<TMessage>>> DelayableQueue { get; set; }

	/// <summary>
	/// <see cref="IMessageBodyProvider"/> is responsible for message body saving
	/// and loading, serialization, encryption and compression
	/// </summary>
	Func<IServiceProvider, IMessageBodyProvider> MessageBodyProvider { get; set; }

	Func<IServiceProvider, IExhcangeRouter >Router { get; set; }

	Func<IServiceProvider, IErrorHandlingController>? ErrorHandling { get; set; }
}
