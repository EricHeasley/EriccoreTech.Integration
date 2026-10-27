using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using EriccoreTech.IntegrationFlow.Model;

namespace EriccoreTech.IntegrationFlow.Messaging {

    // Ephemeral fan-out pub/sub. Each subscription receives its own bounded queue.
    public sealed class InMemoryMessageBus {

        private const int Capacity = 256;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Channel<MessageEnvelope<object>>>> _subscriptions = new();

        public async Task PublishAsync(string topic, MessageEnvelope<object> message, CancellationToken ct = default) {
            if (message.ExpiresAt is { } expires && expires <= DateTimeOffset.UtcNow) {
                return;
            }

            if (!_subscriptions.TryGetValue(topic, out var subscribers)) {
                return;
            }

            foreach (var channel in subscribers.Values) {
                await channel.Writer.WriteAsync(message, ct);
            }
        }

        public async IAsyncEnumerable<MessageEnvelope<object>> Subscribe(
            string topic, [EnumeratorCancellation] CancellationToken ct = default) {
            var id = Guid.NewGuid();
            var subscribers = _subscriptions.GetOrAdd(topic, _ => new());
            var channel = Channel.CreateBounded<MessageEnvelope<object>>(
                new BoundedChannelOptions(Capacity) {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });
            subscribers[id] = channel;
            try {
                await foreach (var message in channel.Reader.ReadAllAsync(ct)) {
                    yield return message;
                }
            } finally {
                subscribers.TryRemove(id, out _);
                channel.Writer.TryComplete();
            }
        }

    }
}
