using System.Text.Json;

namespace EriccoreTech.Integration.Processing {
    public sealed class Aggregator {

        public JsonElement Aggregate(IEnumerable<JsonElement> values) {
            return JsonSerializer.SerializeToElement(values.Select(x => x.Clone()).ToArray());
        }

    }
}
