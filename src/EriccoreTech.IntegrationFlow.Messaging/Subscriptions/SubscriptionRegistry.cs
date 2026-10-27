using System.Collections.Concurrent;

namespace EriccoreTech.IntegrationFlow.Messaging {
    public sealed class SubscriptionRegistry {

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Subscription>> _topics = new();

        public Subscription Add(string topic) {
            var subscription = new Subscription(Guid.NewGuid(), topic, new MessageChannel());
            _topics.GetOrAdd(topic, _ => new()).TryAdd(subscription.Id, subscription);
            return subscription;
        }

        public void Remove(Subscription subscription) {
            if (_topics.TryGetValue(subscription.Topic, out var subscribers)) {
                subscribers.TryRemove(subscription.Id, out _);
            }

            subscription.Channel.Complete();
        }

        public IReadOnlyList<Subscription> For(string topic) {
            if (!_topics.TryGetValue(topic, out var subscribers)) {
                return [];
            }

            return subscribers.Values.ToArray();
        }

    }
}
