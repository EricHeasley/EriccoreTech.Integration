using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record PublishMessageRequest(string Topic, JsonElement Payload);
}
