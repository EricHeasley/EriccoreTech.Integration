using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Persistence;
using EriccoreTech.Integration.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.Integration.Service {
    public sealed class ExecutionHistoryService : IExecutionHistoryService {

        private readonly IntegrationDbContext _db;

        public ExecutionHistoryService(IntegrationDbContext db) {
            _db = db;
        }

        public async Task<ListExecutionsResponse> ListExecutionsAsync(ListExecutionsRequest request, CancellationToken cancellationToken = default) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            var records = await _db.Executions.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(Math.Clamp(request.Limit, 1, 500))
                .ToListAsync(cancellationToken);
            var executions = records.Select(x => new ExecutionSummary(
                x.Id, x.WorkflowId, x.WorkflowVersion, x.Status, x.CreatedAt, x.FinishedAt, x.Error)).ToArray();
            return new ListExecutionsResponse(executions);
        }

    }
}
