namespace EriccoreTech.IntegrationFlow.Persistence {
    public sealed class MessageRecord {

        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MessageId { get; set; }

        public Guid CorrelationId { get; set; }

        public string Topic { get; set; } = "";

        public string MessageType { get; set; } = "";

        public string PayloadJson { get; set; } = "{}";

        public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
