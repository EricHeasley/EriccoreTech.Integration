using System.Runtime.CompilerServices;
using EriccoreTech.IntegrationFlow.Contracts;
using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Messaging {
    public sealed class MessageConsumer : IMessageConsumer {

        private readonly SubscriptionRegistry _registry;

        public MessageConsumer(SubscriptionRegistry registry) {
            _registry = registry;
        }

        public async IAsyncEnumerable<MessageEnvelope<object>> Subscribe(
            string topic, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
            var subscription = _registry.Add(topic);
            try {
                await foreach (var message in subscription.Channel.ReadAllAsync(cancellationToken)) {
                    yield return message;
                }
            } finally {
                _registry.Remove(subscription);
            }
        }

    }
}
