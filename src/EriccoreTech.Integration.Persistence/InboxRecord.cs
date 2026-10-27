namespace EriccoreTech.Integration.Persistence {
    public sealed class InboxRecord {

        public string Consumer { get; set; } = "";

        public Guid MessageId { get; set; }

        public DateTimeOffset ProcessedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
