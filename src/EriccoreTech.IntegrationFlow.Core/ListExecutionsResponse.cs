namespace EriccoreTech.IntegrationFlow.Core {
    public sealed record ListExecutionsResponse(IReadOnlyList<ExecutionSummary> Executions);
}
