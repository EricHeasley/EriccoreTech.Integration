using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Reliability {

    // Runs the handler once per (consumer, message id); duplicates are skipped.
    public sealed class IdempotentHandler {

        private readonly InboxStore _inbox;

        public IdempotentHandler(InboxStore inbox) {
            _inbox = inbox;
        }

        public async Task<bool> HandleAsync(string consumer, MessageEnvelope<object> message,
            Func<MessageEnvelope<object>, Task> handler, CancellationToken cancellationToken = default) {
            if (!await _inbox.TryBeginAsync(consumer, message.MessageId, cancellationToken)) {
                return false;
            }

            await handler(message);
            return true;
        }

    }
}
