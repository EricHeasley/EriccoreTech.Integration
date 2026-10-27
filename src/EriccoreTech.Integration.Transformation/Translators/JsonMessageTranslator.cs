using System.Text.Json;

namespace EriccoreTech.Integration.Transformation {
    public sealed class JsonMessageTranslator {

        public JsonElement Project(JsonElement input, IReadOnlyDictionary<string, string> fields) {
            if (input.ValueKind != JsonValueKind.Object) {
                throw new ArgumentException("Expected JSON object.");
            }

            var output = new Dictionary<string, JsonElement>();
            foreach (var (destination, source) in fields) {
                if (input.TryGetProperty(source, out var value)) {
                    output[destination] = value.Clone();
                }
            }

            return JsonSerializer.SerializeToElement(output);
        }

    }
}
