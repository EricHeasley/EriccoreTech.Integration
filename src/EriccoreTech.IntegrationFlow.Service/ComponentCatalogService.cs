using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Contracts;

namespace EriccoreTech.IntegrationFlow.Service {
    public sealed class ComponentCatalogService : IComponentCatalogService {

        private readonly IEnumerable<IIntegrationComponent> _components;

        public ComponentCatalogService(IEnumerable<IIntegrationComponent> components) {
            _components = components;
        }

        public ListComponentsResponse ListComponents(ListComponentsRequest request) {
            return new ListComponentsResponse(_components.Select(x => x.Type).ToArray());
        }

    }
}
