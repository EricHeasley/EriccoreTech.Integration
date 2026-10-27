using EriccoreTech.IntegrationFlow.Orchestration;
using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Contracts;

namespace EriccoreTech.IntegrationFlow.Service {
    public sealed class WorkflowService : IWorkflowService {

        private readonly ExecutionScheduler _engine;

        public WorkflowService(ExecutionScheduler engine) {
            _engine = engine;
        }

        public async Task<ExecuteWorkflowResponse> ExecuteWorkflowAsync(ExecuteWorkflowRequest request, CancellationToken cancellationToken = default) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            var result = await _engine.ExecuteAsync(request.Definition, request.Input, cancellationToken);
            return new ExecuteWorkflowResponse(result);
        }

    }
}
