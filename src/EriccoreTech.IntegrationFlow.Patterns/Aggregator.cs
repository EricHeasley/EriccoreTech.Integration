using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Patterns {
    public sealed class Aggregator {

        public JsonElement Aggregate(IEnumerable<JsonElement> values) {
            return JsonSerializer.SerializeToElement(values.Select(x => x.Clone()).ToArray());
        }

    }
}
