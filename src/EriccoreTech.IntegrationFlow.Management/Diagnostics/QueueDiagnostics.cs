using EriccoreTech.IntegrationFlow.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.IntegrationFlow.Management {
    public sealed class QueueDiagnostics {

        private readonly IntegrationDbContext _db;

        public QueueDiagnostics(IntegrationDbContext db) {
            _db = db;
        }

        // Queue depth per status (Ready, Leased, Completed, DeadLetter).
        public async Task<IReadOnlyDictionary<string, int>> CountByStatusAsync(CancellationToken cancellationToken = default) {
            var counts = await _db.Queue.AsNoTracking()
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);
            return counts.ToDictionary(x => x.Status, x => x.Count);
        }

    }
}
