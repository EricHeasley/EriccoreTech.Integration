using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Contracts {
    public interface IMessagingService {

        Task<PublishMessageResponse> PublishMessageAsync(PublishMessageRequest request, CancellationToken cancellationToken = default);

    }
}
