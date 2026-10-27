using EriccoreTech.IntegrationFlow.Persistence;
using EriccoreTech.IntegrationFlow.Reliability;
using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.IntegrationFlow.Transport {
    public sealed class SqlQueue {

        private readonly IntegrationDbContext _db;

        public SqlQueue(IntegrationDbContext db) {
            _db = db;
        }

        public async Task EnqueueAsync(string topic, string json, CancellationToken ct = default) {
            _db.Queue.Add(new QueueRecord { Topic = topic, PayloadJson = json });
            await _db.SaveChangesAsync(ct);
        }

        // SQL Server atomic claim: update + OUTPUT, safe across competing consumers.
        public async Task<QueueRecord?> ClaimAsync(string topic, string worker, CancellationToken ct = default) {
            var now = DateTimeOffset.UtcNow;
            var leaseUntil = now.AddMinutes(2);
            var claims = await _db.Database.SqlQueryRaw<QueueClaim>(@"
                ;WITH next_message AS (
                    SELECT TOP (1) * FROM Queue WITH (UPDLOCK, READPAST, ROWLOCK)
                    WHERE Topic = {0} AND AvailableAt <= {1}
                      AND (Status = 'Ready' OR (Status = 'Leased' AND LeaseUntil < {1}))
                    ORDER BY AvailableAt, Id
                )
                UPDATE next_message SET Status = 'Leased', LeaseOwner = {2},
                    LeaseUntil = {3}, Attempts = Attempts + 1
                OUTPUT inserted.Id, inserted.Topic, inserted.PayloadJson,
                    inserted.Attempts, inserted.AvailableAt, inserted.Status,
                    inserted.LeaseOwner, inserted.LeaseUntil, inserted.LastError",
                topic, now, worker, leaseUntil).ToListAsync(ct);
            return claims.Select(x => x.ToRecord()).FirstOrDefault();
        }

        public async Task AckAsync(Guid id, string worker, CancellationToken ct = default) {
            await _db.Queue.Where(x => x.Id == id && x.LeaseOwner == worker && x.Status == "Leased")
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "Completed"), ct);
        }

        public async Task NackAsync(Guid id, string worker, string error, RetryPolicy? retryPolicy = null,
            CancellationToken ct = default) {
            var policy = retryPolicy ?? RetryPolicy.Default;
            var row = await _db.Queue.SingleOrDefaultAsync(x => x.Id == id && x.LeaseOwner == worker, ct);
            if (row is null) {
                return;
            }

            row.Status = new DeadLetterPolicy().StatusAfterFailure(policy, row.Attempts);
            row.AvailableAt = DateTimeOffset.UtcNow.Add(policy.DelayFor(row.Attempts));
            row.LastError = error;
            row.LeaseOwner = null;
            row.LeaseUntil = null;
            await _db.SaveChangesAsync(ct);
        }

    }
}
