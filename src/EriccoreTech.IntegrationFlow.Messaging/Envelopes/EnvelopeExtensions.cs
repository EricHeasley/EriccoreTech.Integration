using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Messaging {
    public static class EnvelopeExtensions {

        public static bool IsExpired<T>(this MessageEnvelope<T> envelope, DateTimeOffset now) {
            return envelope.ExpiresAt is { } expires && expires <= now;
        }

    }
}
