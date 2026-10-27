using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Patterns {
    public sealed class EchoComponent : IIntegrationComponent {

        public string Type => "echo";

        public Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken) {
            return Task.FromResult(StepResult.Ok(context.Input.Clone()));
        }

    }
}
