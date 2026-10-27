using EriccoreTech.IntegrationFlow.Model;

namespace EriccoreTech.IntegrationFlow.Service.Contracts {
    public interface IMessagingService {

        Task<PublishMessageResponse> PublishMessageAsync(PublishMessageRequest request, CancellationToken cancellationToken = default);

    }
}
