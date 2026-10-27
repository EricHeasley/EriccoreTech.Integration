using EriccoreTech.Integration.Contracts;
using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Messaging {

    // Fans a message out to every active subscription on the topic.
    public sealed class MessagePublisher : IMessagePublisher {

        private readonly SubscriptionRegistry _registry;
        private readonly DeliveryPolicy _deliveryPolicy;

        public MessagePublisher(SubscriptionRegistry registry, DeliveryPolicy deliveryPolicy) {
            _registry = registry;
            _deliveryPolicy = deliveryPolicy;
        }

        public async Task PublishAsync(string topic, MessageEnvelope<object> message, CancellationToken cancellationToken = default) {
            if (!_deliveryPolicy.ShouldDeliver(message, DateTimeOffset.UtcNow)) {
                return;
            }

            foreach (var subscription in _registry.For(topic)) {
                await subscription.Channel.WriteAsync(message, cancellationToken);
            }
        }

    }
}
