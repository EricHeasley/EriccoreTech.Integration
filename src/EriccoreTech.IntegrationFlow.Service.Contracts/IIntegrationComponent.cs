using EriccoreTech.IntegrationFlow.Model;

namespace EriccoreTech.IntegrationFlow.Service.Contracts {
    public interface IIntegrationComponent {

        string Type { get; }

        Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken);

    }
}
