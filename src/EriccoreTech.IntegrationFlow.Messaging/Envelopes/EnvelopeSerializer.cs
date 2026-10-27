using System.Text.Json;
using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Messaging {

    // Wire format shared by the broker transports. Payloads come back as JsonElement.
    public static class EnvelopeSerializer {

        public static byte[] Serialize(MessageEnvelope<object> message) {
            return JsonSerializer.SerializeToUtf8Bytes(message);
        }

        public static MessageEnvelope<object> Deserialize(ReadOnlySpan<byte> body) {
            var message = JsonSerializer.Deserialize<MessageEnvelope<object>>(body);
            if (message is null) {
                throw new JsonException("Message body was empty.");
            }

            return message;
        }

    }
}
