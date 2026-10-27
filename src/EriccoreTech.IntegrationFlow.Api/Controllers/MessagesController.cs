using System.Text.Json;
using EriccoreTech.IntegrationFlow.Model;
using EriccoreTech.IntegrationFlow.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EriccoreTech.IntegrationFlow.Api.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase {

        private readonly IMessagingService _messagingService;

        public MessagesController(IMessagingService messagingService) {
            _messagingService = messagingService;
        }

        [HttpPost("{topic}")]
        public async Task<IActionResult> Publish(string topic, [FromBody] JsonElement payload, CancellationToken cancellationToken) {
            var response = await _messagingService.PublishMessageAsync(new PublishMessageRequest(topic, payload), cancellationToken);
            return Accepted(new { response.MessageId, response.CorrelationId });
        }

    }
}
