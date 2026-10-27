using System.Text.Json;

namespace EriccoreTech.Integration.Core {
    public sealed record StepResult(bool Success, JsonElement? Output, string? Error = null) {

        public static StepResult Ok(JsonElement output) {
            return new StepResult(true, output);
        }

        public static StepResult Fail(string error) {
            return new StepResult(false, null, error);
        }

    }
}
