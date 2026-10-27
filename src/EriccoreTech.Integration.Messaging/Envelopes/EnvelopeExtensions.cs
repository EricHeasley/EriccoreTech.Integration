using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Messaging {
    public static class EnvelopeExtensions {

        public static bool IsExpired<T>(this MessageEnvelope<T> envelope, DateTimeOffset now) {
            return envelope.ExpiresAt is { } expires && expires <= now;
        }

    }
}
