using System.Text.Json;
using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Api {
    public sealed record ExecuteRequest(WorkflowDefinition Definition, JsonElement Input);
}
