using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Contracts {
    public interface IMessageConsumer {

        IAsyncEnumerable<MessageEnvelope<object>> Subscribe(string topic, CancellationToken cancellationToken = default);

    }
}
