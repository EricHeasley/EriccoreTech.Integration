using System.Text.Json;
using EriccoreTech.IntegrationFlow.Model;
using EriccoreTech.IntegrationFlow.Service.Contracts;

namespace EriccoreTech.IntegrationFlow.Patterns {
    public sealed class ContentBasedRouterComponent : IIntegrationComponent {

        public string Type => "route.priority";

        public Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken) {
            var source = context.Dependencies.Values.FirstOrDefault()?.Output ?? context.Input;
            var route = source.ValueKind == JsonValueKind.Object &&
                source.TryGetProperty("priority", out var priority) &&
                priority.ValueKind == JsonValueKind.String &&
                string.Equals(priority.GetString(), "high", StringComparison.OrdinalIgnoreCase)
                ? "priority" : "standard";
            return Task.FromResult(StepResult.Ok(JsonSerializer.SerializeToElement(new { route })));
        }

    }
}
