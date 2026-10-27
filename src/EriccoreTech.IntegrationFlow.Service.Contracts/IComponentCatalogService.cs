using EriccoreTech.IntegrationFlow.Model;

namespace EriccoreTech.IntegrationFlow.Service.Contracts {
    public interface IComponentCatalogService {

        ListComponentsResponse ListComponents(ListComponentsRequest request);

    }
}
