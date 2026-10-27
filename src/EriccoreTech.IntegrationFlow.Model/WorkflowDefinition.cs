namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record WorkflowDefinition(string Id, StepDefinition[] Steps);
}
