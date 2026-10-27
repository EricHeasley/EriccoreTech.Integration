using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Contracts {
    public interface IMessageConsumer {

        IAsyncEnumerable<MessageEnvelope<object>> Subscribe(string topic, CancellationToken cancellationToken = default);

    }
}
