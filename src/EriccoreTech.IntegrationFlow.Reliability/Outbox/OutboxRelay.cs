using EriccoreTech.IntegrationFlow.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.IntegrationFlow.Reliability {

    // Moves unpublished outbox rows onto the queue. Single-instance only: there is no row claiming yet.
    public sealed class OutboxRelay {

        private readonly IntegrationDbContext _db;

        public OutboxRelay(IntegrationDbContext db) {
            _db = db;
        }

        public async Task<int> RelayAsync(int batchSize, CancellationToken cancellationToken = default) {
            var batch = await _db.Outbox.Where(x => x.PublishedAt == null)
                .OrderBy(x => x.CreatedAt).Take(batchSize).ToListAsync(cancellationToken);
            foreach (var item in batch) {
                _db.Queue.Add(new QueueRecord { Id = item.Id, Topic = item.Topic, PayloadJson = item.PayloadJson });
                item.PublishedAt = DateTimeOffset.UtcNow;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return batch.Count;
        }

    }
}
