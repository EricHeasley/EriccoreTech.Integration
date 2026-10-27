using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EriccoreTech.Integration.Api {
    [ApiController]
    [Route("[controller]")]
    public class WorkflowsController : ControllerBase {

        private readonly IWorkflowService _workflowService;

        public WorkflowsController(IWorkflowService workflowService) {
            _workflowService = workflowService;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> Execute(ExecuteWorkflowRequest request, CancellationToken cancellationToken) {
            try {
                var response = await _workflowService.ExecuteWorkflowAsync(request, cancellationToken);
                return Ok(response.Result);
            } catch (ArgumentException ex) {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
