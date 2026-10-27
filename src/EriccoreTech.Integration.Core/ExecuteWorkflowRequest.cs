using System.Text.Json;

namespace EriccoreTech.Integration.Core {
    public sealed record ExecuteWorkflowRequest(WorkflowDefinition Definition, JsonElement Input);
}
