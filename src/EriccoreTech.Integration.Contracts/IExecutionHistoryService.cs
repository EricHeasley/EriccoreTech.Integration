using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Contracts {
    public interface IExecutionHistoryService {

        Task<ListExecutionsResponse> ListExecutionsAsync(ListExecutionsRequest request, CancellationToken cancellationToken = default);

    }
}
