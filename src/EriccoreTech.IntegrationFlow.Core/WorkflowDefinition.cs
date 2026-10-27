namespace EriccoreTech.IntegrationFlow.Core {
    public sealed record WorkflowDefinition(string Id, StepDefinition[] Steps);
}
