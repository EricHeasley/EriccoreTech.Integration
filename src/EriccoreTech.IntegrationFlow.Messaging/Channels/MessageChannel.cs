using System.Threading.Channels;
using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Messaging {

    // A bounded, single-reader queue that backs one subscription.
    public sealed class MessageChannel {

        public const int DefaultCapacity = 256;

        private readonly Channel<MessageEnvelope<object>> _channel;

        public MessageChannel(int capacity = DefaultCapacity) {
            _channel = Channel.CreateBounded<MessageEnvelope<object>>(new BoundedChannelOptions(capacity) {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });
        }

        public ValueTask WriteAsync(MessageEnvelope<object> message, CancellationToken cancellationToken) {
            return _channel.Writer.WriteAsync(message, cancellationToken);
        }

        public IAsyncEnumerable<MessageEnvelope<object>> ReadAllAsync(CancellationToken cancellationToken) {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }

        public void Complete() {
            _channel.Writer.TryComplete();
        }

    }
}
