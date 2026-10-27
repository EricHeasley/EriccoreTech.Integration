using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Core {
    public sealed record ExecuteWorkflowRequest(WorkflowDefinition Definition, JsonElement Input);
}
