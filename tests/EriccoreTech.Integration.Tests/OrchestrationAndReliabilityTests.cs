using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Orchestration;
using EriccoreTech.Integration.Persistence;
using EriccoreTech.Integration.Reliability;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EriccoreTech.Integration.Tests {
    public sealed class OrchestrationAndReliabilityTests {

        private static IntegrationDbContext CreateDatabase() {
            var options = new DbContextOptionsBuilder<IntegrationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new IntegrationDbContext(options);
        }

        [Fact]
        public void ProcessManagerEnforcesStateMachine() {
            var manager = new ProcessManager();
            var id = manager.Start();

            Assert.Equal(ProcessStatus.Running, manager.Advance(id, ProcessStatus.Running));
            Assert.Equal(ProcessStatus.Completed, manager.Advance(id, ProcessStatus.Completed));
            Assert.Throws<InvalidOperationException>(() => manager.Advance(id, ProcessStatus.Running));
            Assert.Equal(ProcessStatus.Completed, manager.Get(id));
        }

        [Fact]
        public void RetryPolicyBacksOffExponentiallyUpToTheCap() {
            var policy = new RetryPolicy(5, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10));

            Assert.Equal(TimeSpan.FromSeconds(2), policy.DelayFor(1));
            Assert.Equal(TimeSpan.FromSeconds(4), policy.DelayFor(2));
            Assert.Equal(TimeSpan.FromSeconds(10), policy.DelayFor(10));
            Assert.False(policy.IsExhausted(4));
            Assert.True(policy.IsExhausted(5));
        }

        [Fact]
        public void DeadLetterPolicyDeadLettersWhenRetriesAreExhausted() {
            var retry = new RetryPolicy(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            var policy = new DeadLetterPolicy();

            Assert.Equal("Ready", policy.StatusAfterFailure(retry, 2));
            Assert.Equal("DeadLetter", policy.StatusAfterFailure(retry, 3));
        }

        [Fact]
        public async Task InboxStoreAcceptsEachMessageOncePerConsumer() {
            await using var db = CreateDatabase();
            var inbox = new InboxStore(db);
            var messageId = Guid.NewGuid();

            Assert.True(await inbox.TryBeginAsync("billing", messageId));
            Assert.False(await inbox.TryBeginAsync("billing", messageId));
            Assert.True(await inbox.TryBeginAsync("shipping", messageId));
        }

        [Fact]
        public async Task IdempotentHandlerRunsHandlerOnce() {
            await using var db = CreateDatabase();
            var handler = new IdempotentHandler(new InboxStore(db));
            var message = MessageEnvelope<object>.Create("jobs", "payload");
            var calls = 0;

            Assert.True(await handler.HandleAsync("billing", message, _ => {
                calls++;
                return Task.CompletedTask;
            }));
            Assert.False(await handler.HandleAsync("billing", message, _ => {
                calls++;
                return Task.CompletedTask;
            }));

            Assert.Equal(1, calls);
        }

        [Fact]
        public async Task OutboxRelayMovesRowsToQueueOnce() {
            await using var db = CreateDatabase();
            await new OutboxStore(db).AddAsync("jobs", "{}", CancellationToken.None);
            var relay = new OutboxRelay(db);

            Assert.Equal(1, await relay.RelayAsync(25));
            Assert.Equal(0, await relay.RelayAsync(25));
            Assert.Single(db.Queue);
        }

    }
}
