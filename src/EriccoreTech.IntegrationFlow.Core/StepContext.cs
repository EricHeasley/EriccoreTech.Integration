using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Core {
    public sealed record StepContext(Guid ExecutionId, JsonElement Input,
        IReadOnlyDictionary<string, StepResult> Dependencies);
}
