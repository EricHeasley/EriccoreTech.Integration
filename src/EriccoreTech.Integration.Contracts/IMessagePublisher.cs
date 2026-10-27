using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Contracts {
    public interface IMessagePublisher {

        Task PublishAsync(string topic, MessageEnvelope<object> message, CancellationToken cancellationToken = default);

    }
}
