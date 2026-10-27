using System.Text.Json;

namespace EriccoreTech.Integration.Core {
    public sealed record StepContext(Guid ExecutionId, JsonElement Input,
        IReadOnlyDictionary<string, StepResult> Dependencies);
}
