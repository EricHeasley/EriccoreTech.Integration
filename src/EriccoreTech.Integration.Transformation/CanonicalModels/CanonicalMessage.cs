using System.Text.Json;

namespace EriccoreTech.Integration.Transformation {
    public sealed record CanonicalMessage(string Type, int Version, JsonElement Data);
}
