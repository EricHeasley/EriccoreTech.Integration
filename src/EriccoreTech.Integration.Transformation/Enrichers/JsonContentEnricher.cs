using System.Text.Json;

namespace EriccoreTech.Integration.Transformation {

    // Adds fields from a lookup. Existing message fields always win over enrichment values.
    public sealed class JsonContentEnricher {

        public JsonElement Enrich(JsonElement message, IReadOnlyDictionary<string, JsonElement> additions) {
            if (message.ValueKind != JsonValueKind.Object) {
                throw new ArgumentException("Expected JSON object.");
            }

            var output = new Dictionary<string, JsonElement>();
            foreach (var (key, value) in additions) {
                output[key] = value.Clone();
            }

            foreach (var property in message.EnumerateObject()) {
                output[property.Name] = property.Value.Clone();
            }

            return JsonSerializer.SerializeToElement(output);
        }

    }
}
