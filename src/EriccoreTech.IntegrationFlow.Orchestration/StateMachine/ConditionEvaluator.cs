using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Orchestration {
    public static class ConditionEvaluator {

        public static bool Evaluate(JsonElement input, Condition condition) {
            if (input.ValueKind != JsonValueKind.Object ||
                !input.TryGetProperty(condition.Field, out var value)) {
                return false;
            }

            var actual = value.ToString();
            return condition.Operator.ToLowerInvariant() switch {
                "eq" => string.Equals(actual, condition.Value, StringComparison.OrdinalIgnoreCase),
                "neq" => !string.Equals(actual, condition.Value, StringComparison.OrdinalIgnoreCase),
                "exists" => true,
                _ => throw new ArgumentException($"Unknown condition operator: {condition.Operator}")
            };
        }

    }
}
