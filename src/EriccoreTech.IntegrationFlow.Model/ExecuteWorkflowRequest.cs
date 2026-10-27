using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record ExecuteWorkflowRequest(WorkflowDefinition Definition, JsonElement Input);
}
