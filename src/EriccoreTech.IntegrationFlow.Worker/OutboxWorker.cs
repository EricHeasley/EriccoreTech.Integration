using EriccoreTech.IntegrationFlow.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.IntegrationFlow.Worker {
    public sealed class OutboxWorker : BackgroundService {

        private readonly IServiceScopeFactory _scopeFactory;

        public OutboxWorker(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<IntegrationDbContext>();
                    // Single-instance demonstration only; production needs row claiming/locking.
                    var batch = await db.Outbox.Where(x => x.PublishedAt == null)
                        .OrderBy(x => x.CreatedAt).Take(25).ToListAsync(stoppingToken);
                    foreach (var item in batch) {
                        db.Queue.Add(new QueueRecord { Id = item.Id, Topic = item.Topic, PayloadJson = item.PayloadJson });
                        item.PublishedAt = DateTimeOffset.UtcNow;
                    }

                    await db.SaveChangesAsync(stoppingToken);
                } catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                    break;
                } catch (Exception ex) {
                    Console.Error.WriteLine($"Outbox processing failed: {ex}");
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

    }
}
