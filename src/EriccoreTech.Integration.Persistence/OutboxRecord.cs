namespace EriccoreTech.Integration.Persistence {
    public sealed class OutboxRecord {

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Topic { get; set; } = "";

        public string PayloadJson { get; set; } = "{}";

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? PublishedAt { get; set; }

    }
}
