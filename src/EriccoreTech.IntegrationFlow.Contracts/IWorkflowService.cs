using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Contracts {
    public interface IWorkflowService {

        Task<ExecuteWorkflowResponse> ExecuteWorkflowAsync(ExecuteWorkflowRequest request, CancellationToken cancellationToken = default);

    }
}
