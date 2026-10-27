namespace EriccoreTech.IntegrationFlow.Persistence {
    public sealed class ExecutionRecord {

        public Guid Id { get; set; }

        public string WorkflowId { get; set; } = "";

        public int WorkflowVersion { get; set; } = 1;

        public string InputJson { get; set; } = "{}";

        public string Status { get; set; } = "Queued";

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? FinishedAt { get; set; }

        public string? Error { get; set; }

    }
}
