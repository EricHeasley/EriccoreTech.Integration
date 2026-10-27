using EriccoreTech.IntegrationFlow.Persistence;

namespace EriccoreTech.IntegrationFlow.Transport {
    public sealed class QueueClaim {

        public Guid Id { get; set; }

        public string Topic { get; set; } = "";

        public string PayloadJson { get; set; } = "";

        public int Attempts { get; set; }

        public DateTimeOffset AvailableAt { get; set; }

        public string Status { get; set; } = "";

        public string? LeaseOwner { get; set; }

        public DateTimeOffset? LeaseUntil { get; set; }

        public string? LastError { get; set; }

        public QueueRecord ToRecord() {
            return new QueueRecord {
                Id = Id,
                Topic = Topic,
                PayloadJson = PayloadJson,
                Attempts = Attempts,
                AvailableAt = AvailableAt,
                Status = Status,
                LeaseOwner = LeaseOwner,
                LeaseUntil = LeaseUntil,
                LastError = LastError
            };
        }

    }
}
