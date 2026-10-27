using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EriccoreTech.Integration.Api {
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
