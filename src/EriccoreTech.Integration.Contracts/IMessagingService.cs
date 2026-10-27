using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Contracts {
    public interface IMessagingService {

        Task<PublishMessageResponse> PublishMessageAsync(PublishMessageRequest request, CancellationToken cancellationToken = default);

    }
}
