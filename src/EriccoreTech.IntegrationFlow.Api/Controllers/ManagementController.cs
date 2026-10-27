using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EriccoreTech.IntegrationFlow.Api {
    [ApiController]
    [Route("[controller]/channels")]
    public class ManagementController : ControllerBase {

        private readonly IManagementService _managementService;

        public ManagementController(IManagementService managementService) {
            _managementService = managementService;
        }

        [HttpGet]
        public IActionResult List() {
            var response = _managementService.ListChannels(new ListChannelsRequest());
            return Ok(response.Channels);
        }

        [HttpPost("{topic}/pause")]
        public IActionResult Pause(string topic) {
            _managementService.PauseChannel(new PauseChannelRequest(topic));
            return Ok();
        }

        [HttpPost("{topic}/resume")]
        public IActionResult Resume(string topic) {
            _managementService.ResumeChannel(new ResumeChannelRequest(topic));
            return Ok();
        }

    }
}
