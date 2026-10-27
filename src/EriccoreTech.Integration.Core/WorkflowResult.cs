namespace EriccoreTech.Integration.Core {
    public sealed record WorkflowResult(Guid ExecutionId, IReadOnlyDictionary<string, StepResult> Steps);
}
