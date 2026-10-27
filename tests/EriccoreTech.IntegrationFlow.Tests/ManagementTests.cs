using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Management;
using EriccoreTech.IntegrationFlow.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EriccoreTech.IntegrationFlow.Tests {
    public sealed class ManagementTests {

        private static IntegrationDbContext CreateDatabase() {
            var options = new DbContextOptionsBuilder<IntegrationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new IntegrationDbContext(options);
        }

        [Fact]
        public async Task MessageStoreRecordsAndHistoryFindsByCorrelation() {
            await using var db = CreateDatabase();
            var correlationId = Guid.NewGuid();
            var store = new MessageStore(db);
            await store.RecordAsync("jobs", MessageEnvelope<object>.Create("jobs", "a", correlationId));
            await store.RecordAsync("jobs", MessageEnvelope<object>.Create("jobs", "b", correlationId));
            await store.RecordAsync("other", MessageEnvelope<object>.Create("other", "c"));

            var history = new MessageHistoryReader(db);

            Assert.Equal(2, (await history.ForCorrelationAsync(correlationId)).Count);
            Assert.Equal(2, (await history.RecentForTopicAsync("jobs", 10)).Count);
        }

        [Fact]
        public async Task QueueDiagnosticsCountsRowsByStatus() {
            await using var db = CreateDatabase();
            db.Queue.Add(new QueueRecord { Status = "Ready" });
            db.Queue.Add(new QueueRecord { Status = "Ready" });
            db.Queue.Add(new QueueRecord { Status = "DeadLetter" });
            await db.SaveChangesAsync();

            var counts = await new QueueDiagnostics(db).CountByStatusAsync();

            Assert.Equal(2, counts["Ready"]);
            Assert.Equal(1, counts["DeadLetter"]);
        }

    }
}
