using System.Text.Json;
using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Contracts;

namespace EriccoreTech.Integration.Transformation {
    public sealed class UppercaseComponent : IIntegrationComponent {

        public string Type => "uppercase";

        public Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken) {
            var source = context.Dependencies.Values.FirstOrDefault()?.Output ?? context.Input;
            var value = source.ValueKind == JsonValueKind.Object &&
                source.TryGetProperty("text", out var text) ? text.GetString() : source.ToString();
            return Task.FromResult(StepResult.Ok(JsonSerializer.SerializeToElement(
                new { text = (value ?? "").ToUpperInvariant() })));
        }

    }
}
