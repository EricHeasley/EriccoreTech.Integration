namespace EriccoreTech.Integration.Persistence {
    public sealed class StepRecord {

        public Guid ExecutionId { get; set; }

        public string StepId { get; set; } = "";

        public string Status { get; set; } = "Pending";

        public int Attempts { get; set; }

        public string? OutputJson { get; set; }

        public string? Error { get; set; }

    }
}
