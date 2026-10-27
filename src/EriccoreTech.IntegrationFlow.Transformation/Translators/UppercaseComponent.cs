using System.Text.Json;
using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Contracts;

namespace EriccoreTech.IntegrationFlow.Transformation {
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
