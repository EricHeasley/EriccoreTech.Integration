using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Transformation {
    public sealed record CanonicalMessage(string Type, int Version, JsonElement Data);
}
