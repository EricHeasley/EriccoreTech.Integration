namespace EriccoreTech.IntegrationFlow.Core {
    public interface IIntegrationComponent {

        string Type { get; }

        Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken);

    }
}
