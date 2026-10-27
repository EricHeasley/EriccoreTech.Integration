namespace EriccoreTech.IntegrationFlow.Persistence {
    public sealed class QueueRecord {

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Topic { get; set; } = "";

        public string PayloadJson { get; set; } = "{}";

        public int Attempts { get; set; }

        public DateTimeOffset AvailableAt { get; set; } = DateTimeOffset.UtcNow;

        public string Status { get; set; } = "Ready";

        public string? LeaseOwner { get; set; }

        public DateTimeOffset? LeaseUntil { get; set; }

        public string? LastError { get; set; }

    }
}
