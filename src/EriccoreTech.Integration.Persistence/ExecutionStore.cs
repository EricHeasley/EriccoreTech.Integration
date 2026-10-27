using System.Text.Json;
using EriccoreTech.Integration.Core;
using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.Integration.Persistence {
    public sealed class ExecutionStore {

        private readonly IntegrationDbContext _db;

        public ExecutionStore(IntegrationDbContext db) {
            _db = db;
        }

        public async Task QueueAsync(Guid id, string workflowId, int version, JsonElement input, CancellationToken ct) {
            _db.Executions.Add(new ExecutionRecord {
                Id = id,
                WorkflowId = workflowId,
                WorkflowVersion = version,
                InputJson = input.GetRawText()
            });
            await _db.SaveChangesAsync(ct);
        }

        public async Task CompleteAsync(Guid id, WorkflowResult result, CancellationToken ct) {
            var execution = await _db.Executions.SingleAsync(x => x.Id == id, ct);
            execution.Status = result.Steps.Values.All(x => x.Success) ? "Completed" : "Failed";
            execution.FinishedAt = DateTimeOffset.UtcNow;
            foreach (var (key, value) in result.Steps) {
                _db.Steps.Add(new StepRecord {
                    ExecutionId = id,
                    StepId = key,
                    Status = value.Success ? "Completed" : "Failed",
                    OutputJson = value.Output?.GetRawText(),
                    Error = value.Error
                });
            }

            await _db.SaveChangesAsync(ct);
        }

    }
}
