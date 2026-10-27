using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.Integration.Persistence {
    public sealed class IntegrationDbContext : DbContext {

        public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options) {
        }

        public DbSet<ExecutionRecord> Executions => Set<ExecutionRecord>();

        public DbSet<StepRecord> Steps => Set<StepRecord>();

        public DbSet<OutboxRecord> Outbox => Set<OutboxRecord>();

        public DbSet<InboxRecord> Inbox => Set<InboxRecord>();

        public DbSet<QueueRecord> Queue => Set<QueueRecord>();

        public DbSet<WorkflowRecord> Workflows => Set<WorkflowRecord>();

        public DbSet<MessageRecord> Messages => Set<MessageRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ExecutionRecord>().HasKey(x => x.Id);
            modelBuilder.Entity<StepRecord>().HasKey(x => new { x.ExecutionId, x.StepId });
            modelBuilder.Entity<OutboxRecord>().HasKey(x => x.Id);
            modelBuilder.Entity<InboxRecord>().HasKey(x => new { x.Consumer, x.MessageId });
            modelBuilder.Entity<QueueRecord>().HasKey(x => x.Id);
            modelBuilder.Entity<QueueRecord>().HasIndex(x => new { x.Topic, x.AvailableAt });
            modelBuilder.Entity<WorkflowRecord>().HasKey(x => new { x.Id, x.Version });
            modelBuilder.Entity<MessageRecord>().HasKey(x => x.Id);
            modelBuilder.Entity<MessageRecord>().HasIndex(x => x.CorrelationId);
        }

    }
}
