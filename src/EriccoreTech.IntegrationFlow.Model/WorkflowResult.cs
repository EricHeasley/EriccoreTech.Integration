namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record WorkflowResult(Guid ExecutionId, IReadOnlyDictionary<string, StepResult> Steps);
}
