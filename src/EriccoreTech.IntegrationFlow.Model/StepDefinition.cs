namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record StepDefinition(string Id, string Component, string[]? DependsOn = null,
        int MaxAttempts = 1, int TimeoutSeconds = 30);
}
