using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Messaging {
    public sealed class DeliveryPolicy {

        public bool ShouldDeliver<T>(MessageEnvelope<T> message, DateTimeOffset now) {
            return !message.IsExpired(now);
        }

    }
}
