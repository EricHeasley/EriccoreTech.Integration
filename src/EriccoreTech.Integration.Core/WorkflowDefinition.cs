namespace EriccoreTech.Integration.Core {
    public sealed record WorkflowDefinition(string Id, StepDefinition[] Steps);
}
