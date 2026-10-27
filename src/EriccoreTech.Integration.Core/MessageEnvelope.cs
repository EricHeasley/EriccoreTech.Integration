namespace EriccoreTech.Integration.Core {
    public sealed record MessageEnvelope<T>(
        Guid MessageId, Guid CorrelationId, Guid? CausationId,
        string MessageType, int SchemaVersion, DateTimeOffset CreatedAt,
        DateTimeOffset? ExpiresAt, IReadOnlyDictionary<string, string> Headers, T Payload) {

        public static MessageEnvelope<T> Create(string type, T payload, Guid? correlationId = null) {
            return new MessageEnvelope<T>(Guid.NewGuid(), correlationId ?? Guid.NewGuid(), null, type, 1,
                DateTimeOffset.UtcNow, null, new Dictionary<string, string>(), payload);
        }

    }
}
