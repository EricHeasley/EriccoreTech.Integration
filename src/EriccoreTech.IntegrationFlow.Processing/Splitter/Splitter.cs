using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Processing {
    public sealed class Splitter {

        public IReadOnlyList<JsonElement> Split(JsonElement array) {
            if (array.ValueKind != JsonValueKind.Array) {
                throw new ArgumentException("Expected a JSON array.");
            }

            return array.EnumerateArray().Select(x => x.Clone()).ToArray();
        }

    }
}
