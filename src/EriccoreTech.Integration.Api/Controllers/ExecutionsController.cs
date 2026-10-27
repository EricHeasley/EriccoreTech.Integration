using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EriccoreTech.Integration.Api {
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
