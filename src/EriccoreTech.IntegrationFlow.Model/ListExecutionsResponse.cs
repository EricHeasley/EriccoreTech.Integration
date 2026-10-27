namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record ListExecutionsResponse(IReadOnlyList<ExecutionSummary> Executions);
}
