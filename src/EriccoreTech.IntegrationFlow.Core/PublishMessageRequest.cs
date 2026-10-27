using System.Text.Json;

namespace EriccoreTech.IntegrationFlow.Core {
    public sealed record PublishMessageRequest(string Topic, JsonElement Payload);
}
