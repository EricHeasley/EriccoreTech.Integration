using EriccoreTech.IntegrationFlow.Model;
using EriccoreTech.IntegrationFlow.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EriccoreTech.IntegrationFlow.Api.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ExecutionsController : ControllerBase {

        private readonly IExecutionHistoryService _executionHistoryService;

        public ExecutionsController(IExecutionHistoryService executionHistoryService) {
            _executionHistoryService = executionHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken) {
            var response = await _executionHistoryService.ListExecutionsAsync(new ListExecutionsRequest(), cancellationToken);
            return Ok(response.Executions);
        }

    }
}
