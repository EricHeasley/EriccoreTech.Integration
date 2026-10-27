using System.Text.Json;

namespace EriccoreTech.Integration.Core {
    public sealed record PublishMessageRequest(string Topic, JsonElement Payload);
}
