namespace EriccoreTech.Integration.Core {
    public sealed record ListExecutionsResponse(IReadOnlyList<ExecutionSummary> Executions);
}
