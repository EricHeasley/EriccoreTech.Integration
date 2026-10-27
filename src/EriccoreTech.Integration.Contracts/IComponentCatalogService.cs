using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Contracts {
    public interface IComponentCatalogService {

        ListComponentsResponse ListComponents(ListComponentsRequest request);

    }
}
