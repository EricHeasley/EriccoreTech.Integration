using EriccoreTech.IntegrationFlow.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.IntegrationFlow.Management {
    public sealed class MessageHistoryReader {

        private readonly IntegrationDbContext _db;

        public MessageHistoryReader(IntegrationDbContext db) {
            _db = db;
        }

        public async Task<IReadOnlyList<MessageRecord>> ForCorrelationAsync(Guid correlationId, CancellationToken cancellationToken = default) {
            return await _db.Messages.AsNoTracking()
                .Where(x => x.CorrelationId == correlationId)
                .OrderBy(x => x.RecordedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<MessageRecord>> RecentForTopicAsync(string topic, int limit, CancellationToken cancellationToken = default) {
            return await _db.Messages.AsNoTracking()
                .Where(x => x.Topic == topic)
                .OrderByDescending(x => x.RecordedAt)
                .Take(Math.Clamp(limit, 1, 500))
                .ToListAsync(cancellationToken);
        }

    }
}
