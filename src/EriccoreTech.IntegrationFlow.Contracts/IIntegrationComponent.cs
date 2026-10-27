using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Contracts {
    public interface IIntegrationComponent {

        string Type { get; }

        Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken);

    }
}
