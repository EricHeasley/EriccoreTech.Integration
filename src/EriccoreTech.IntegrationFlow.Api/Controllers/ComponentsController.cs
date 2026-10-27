using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EriccoreTech.IntegrationFlow.Api {
    [ApiController]
    [Route("[controller]")]
    public class ComponentsController : ControllerBase {

        private readonly IComponentCatalogService _componentCatalogService;

        public ComponentsController(IComponentCatalogService componentCatalogService) {
            _componentCatalogService = componentCatalogService;
        }

        [HttpGet]
        public IActionResult Get() {
            var response = _componentCatalogService.ListComponents(new ListComponentsRequest());
            return Ok(response.ComponentTypes);
        }

    }
}
