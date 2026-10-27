using EriccoreTech.Integration.Orchestration;
using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Contracts;

namespace EriccoreTech.Integration.Service {
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
