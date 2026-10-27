using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Contracts {
    public interface IMessagePublisher {

        Task PublishAsync(string topic, MessageEnvelope<object> message, CancellationToken cancellationToken = default);

    }
}
