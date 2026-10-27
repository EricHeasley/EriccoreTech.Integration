using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Messaging {
    public sealed class DeliveryPolicy {

        public bool ShouldDeliver<T>(MessageEnvelope<T> message, DateTimeOffset now) {
            return !message.IsExpired(now);
        }

    }
}
