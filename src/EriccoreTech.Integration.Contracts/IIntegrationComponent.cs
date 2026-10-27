using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Contracts {
    public interface IIntegrationComponent {

        string Type { get; }

        Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken);

    }
}
