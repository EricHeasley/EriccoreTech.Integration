using System.Text.Json;

namespace EriccoreTech.Integration.Routing {
    public sealed class RecipientListRouter {

        public IReadOnlyList<string> Route(JsonElement message, IReadOnlyDictionary<string, string> rules) {
            if (message.ValueKind != JsonValueKind.Object) {
                return [];
            }

            return rules.Where(rule => message.TryGetProperty(rule.Key, out var value) &&
                string.Equals(value.ToString(), rule.Value, StringComparison.OrdinalIgnoreCase))
                .Select(rule => rule.Key).ToArray();
        }

    }
}
