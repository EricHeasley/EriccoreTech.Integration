using EriccoreTech.IntegrationFlow.Engine;
using EriccoreTech.IntegrationFlow.Model;
using EriccoreTech.IntegrationFlow.Service.Contracts;

namespace EriccoreTech.IntegrationFlow.Service {
    public sealed class WorkflowService : IWorkflowService {

        private readonly WorkflowEngine _engine;

        public WorkflowService(WorkflowEngine engine) {
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
