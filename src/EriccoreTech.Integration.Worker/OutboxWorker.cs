using EriccoreTech.Integration.Reliability;

namespace EriccoreTech.Integration.Worker {
    public sealed class OutboxWorker : BackgroundService {

        private const int BatchSize = 25;

        private readonly IServiceScopeFactory _scopeFactory;

        public OutboxWorker(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    using var scope = _scopeFactory.CreateScope();
                    var relay = scope.ServiceProvider.GetRequiredService<OutboxRelay>();
                    await relay.RelayAsync(BatchSize, stoppingToken);
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
