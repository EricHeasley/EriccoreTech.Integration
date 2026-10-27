using EriccoreTech.IntegrationFlow.Model;

namespace EriccoreTech.IntegrationFlow.Service.Contracts {
    public interface IWorkflowService {

        Task<ExecuteWorkflowResponse> ExecuteWorkflowAsync(ExecuteWorkflowRequest request, CancellationToken cancellationToken = default);

    }
}
