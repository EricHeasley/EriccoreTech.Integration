using EriccoreTech.Integration.Contracts;
using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Service {
    public sealed class MessagingService : IMessagingService {

        private readonly IMessagePublisher _publisher;

        public MessagingService(IMessagePublisher publisher) {
            _publisher = publisher;
        }

        public async Task<PublishMessageResponse> PublishMessageAsync(PublishMessageRequest request, CancellationToken cancellationToken = default) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            var envelope = MessageEnvelope<object>.Create(request.Topic, request.Payload.Clone());
            await _publisher.PublishAsync(request.Topic, envelope, cancellationToken);
            return new PublishMessageResponse(envelope.MessageId, envelope.CorrelationId);
        }

    }
}
