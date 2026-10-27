using EriccoreTech.IntegrationFlow.Messaging;
using EriccoreTech.IntegrationFlow.Model;
using EriccoreTech.IntegrationFlow.Service.Contracts;

namespace EriccoreTech.IntegrationFlow.Service {
    public sealed class MessagingService : IMessagingService {

        private readonly InMemoryMessageBus _bus;

        public MessagingService(InMemoryMessageBus bus) {
            _bus = bus;
        }

        public async Task<PublishMessageResponse> PublishMessageAsync(PublishMessageRequest request, CancellationToken cancellationToken = default) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            var envelope = MessageEnvelope<object>.Create(request.Topic, request.Payload.Clone());
            await _bus.PublishAsync(request.Topic, envelope, cancellationToken);
            return new PublishMessageResponse(envelope.MessageId, envelope.CorrelationId);
        }

    }
}
