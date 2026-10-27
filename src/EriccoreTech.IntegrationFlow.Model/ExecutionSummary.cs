namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record ExecutionSummary(Guid Id, string WorkflowId, int WorkflowVersion, string Status,
        DateTimeOffset CreatedAt, DateTimeOffset? FinishedAt, string? Error);
}
