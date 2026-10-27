using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Contracts {
    public interface IWorkflowService {

        Task<ExecuteWorkflowResponse> ExecuteWorkflowAsync(ExecuteWorkflowRequest request, CancellationToken cancellationToken = default);

    }
}
