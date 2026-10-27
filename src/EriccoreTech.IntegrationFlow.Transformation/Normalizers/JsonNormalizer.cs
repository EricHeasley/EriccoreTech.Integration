using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Transformation {

    // Maps differently shaped inputs onto one canonical property name using alias lists.
    public sealed class JsonNormalizer {

        public JsonElement Normalize(JsonElement input, IReadOnlyDictionary<string, string[]> aliases) {
            if (input.ValueKind != JsonValueKind.Object) {
                throw new ArgumentException("Expected JSON object.");
            }

            var output = new Dictionary<string, JsonElement>();
            foreach (var (canonical, names) in aliases) {
                foreach (var name in names.Prepend(canonical)) {
                    if (input.TryGetProperty(name, out var value)) {
                        output[canonical] = value.Clone();
                        break;
                    }
                }
            }

            return JsonSerializer.SerializeToElement(output);
        }

    }
}
