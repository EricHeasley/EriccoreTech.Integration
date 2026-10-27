using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Contracts {
    public interface IComponentCatalogService {

        ListComponentsResponse ListComponents(ListComponentsRequest request);

    }
}
