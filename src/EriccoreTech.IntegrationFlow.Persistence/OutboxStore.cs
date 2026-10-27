namespace EriccoreTech.IntegrationFlow.Persistence {
    public sealed class OutboxStore {

        private readonly IntegrationDbContext _db;

        public OutboxStore(IntegrationDbContext db) {
            _db = db;
        }

        public async Task AddAsync(string topic, string payload, CancellationToken ct) {
            _db.Outbox.Add(new OutboxRecord { Topic = topic, PayloadJson = payload });
            // Call SaveChanges in the SAME DbContext transaction as the business change.
            await _db.SaveChangesAsync(ct);
        }

    }
}
