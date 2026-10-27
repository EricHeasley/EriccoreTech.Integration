using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Contracts {
    public interface IExecutionHistoryService {

        Task<ListExecutionsResponse> ListExecutionsAsync(ListExecutionsRequest request, CancellationToken cancellationToken = default);

    }
}
