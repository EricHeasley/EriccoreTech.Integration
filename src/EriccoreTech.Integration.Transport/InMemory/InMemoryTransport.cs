using EriccoreTech.Integration.Contracts;
using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Messaging;

namespace EriccoreTech.Integration.Transport {

    // Ephemeral in-process fan-out. Subscribers only see messages published after they subscribe.
    public sealed class InMemoryTransport : IMessageTransport {

        private readonly MessagePublisher _publisher;
        private readonly MessageConsumer _consumer;

        public InMemoryTransport() {
            var registry = new SubscriptionRegistry();
            _publisher = new MessagePublisher(registry, new DeliveryPolicy());
            _consumer = new MessageConsumer(registry);
        }

        public Task PublishAsync(string topic, MessageEnvelope<object> message, CancellationToken cancellationToken = default) {
            return _publisher.PublishAsync(topic, message, cancellationToken);
        }

        public IAsyncEnumerable<MessageEnvelope<object>> Subscribe(string topic, CancellationToken cancellationToken = default) {
            return _consumer.Subscribe(topic, cancellationToken);
        }

    }
}
