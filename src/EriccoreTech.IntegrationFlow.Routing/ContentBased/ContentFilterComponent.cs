using System.Text.Json;
using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Contracts;

namespace EriccoreTech.IntegrationFlow.Routing {
    public sealed class ContentFilterComponent : IIntegrationComponent {

        public string Type => "filter.nonempty";

        public Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken) {
            var source = context.Dependencies.Values.FirstOrDefault()?.Output ?? context.Input;
            if (source.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined) {
                return Task.FromResult(StepResult.Fail("Filtered empty payload."));
            }

            return Task.FromResult(StepResult.Ok(source.Clone()));
        }

    }
}
