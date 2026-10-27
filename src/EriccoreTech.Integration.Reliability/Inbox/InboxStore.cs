using EriccoreTech.Integration.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EriccoreTech.Integration.Reliability {
    public sealed class InboxStore {

        private readonly IntegrationDbContext _db;

        public InboxStore(IntegrationDbContext db) {
            _db = db;
        }

        // Returns false when this consumer has already processed the message.
        public async Task<bool> TryBeginAsync(string consumer, Guid messageId, CancellationToken cancellationToken = default) {
            if (await _db.Inbox.AnyAsync(x => x.Consumer == consumer && x.MessageId == messageId, cancellationToken)) {
                return false;
            }

            _db.Inbox.Add(new InboxRecord { Consumer = consumer, MessageId = messageId });
            try {
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            } catch (DbUpdateException) {
                // A competing consumer recorded the same message between the check and the insert.
                return false;
            }
        }

    }
}
