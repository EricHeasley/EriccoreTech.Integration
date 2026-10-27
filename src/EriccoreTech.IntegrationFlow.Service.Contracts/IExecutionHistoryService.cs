using EriccoreTech.IntegrationFlow.Model;

namespace EriccoreTech.IntegrationFlow.Service.Contracts {
    public interface IExecutionHistoryService {

        Task<ListExecutionsResponse> ListExecutionsAsync(ListExecutionsRequest request, CancellationToken cancellationToken = default);

    }
}
