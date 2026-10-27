using System.Text.Json;
using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Persistence;

namespace EriccoreTech.Integration.Management {

    // Wire Tap target: records a copy of every message that passes a channel.
    public sealed class MessageStore {

        private readonly IntegrationDbContext _db;

        public MessageStore(IntegrationDbContext db) {
            _db = db;
        }

        public async Task RecordAsync(string topic, MessageEnvelope<object> message, CancellationToken cancellationToken = default) {
            _db.Messages.Add(new MessageRecord {
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                Topic = topic,
                MessageType = message.MessageType,
                PayloadJson = JsonSerializer.Serialize(message.Payload)
            });
            await _db.SaveChangesAsync(cancellationToken);
        }

    }
}
