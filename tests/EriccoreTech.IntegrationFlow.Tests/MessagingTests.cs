using System.Text.Json;
using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Messaging;
using EriccoreTech.IntegrationFlow.Transport;
using Xunit;

namespace EriccoreTech.IntegrationFlow.Tests {
    public sealed class MessagingTests {

        [Fact]
        public async Task InMemoryTransportFansOutToEverySubscriber() {
            var transport = new InMemoryTransport();
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var first = transport.Subscribe("jobs", cancellation.Token).GetAsyncEnumerator(cancellation.Token);
            var second = transport.Subscribe("jobs", cancellation.Token).GetAsyncEnumerator(cancellation.Token);
            var firstRead = first.MoveNextAsync().AsTask();
            var secondRead = second.MoveNextAsync().AsTask();
            await Task.Delay(100);

            var message = MessageEnvelope<object>.Create("jobs", "payload");
            await transport.PublishAsync("jobs", message);

            Assert.True(await firstRead);
            Assert.True(await secondRead);
            Assert.Equal(message.MessageId, first.Current.MessageId);
            Assert.Equal(message.MessageId, second.Current.MessageId);
            await first.DisposeAsync();
            await second.DisposeAsync();
        }

        [Fact]
        public async Task ExpiredMessagesAreNotDelivered() {
            var registry = new SubscriptionRegistry();
            var subscription = registry.Add("jobs");
            var publisher = new MessagePublisher(registry, new DeliveryPolicy());
            var expired = MessageEnvelope<object>.Create("jobs", "payload") with { ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-1) };

            await publisher.PublishAsync("jobs", expired);
            registry.Remove(subscription);

            var delivered = new List<MessageEnvelope<object>>();
            await foreach (var message in subscription.Channel.ReadAllAsync(CancellationToken.None)) {
                delivered.Add(message);
            }

            Assert.Empty(delivered);
        }

        [Fact]
        public void EnvelopeSerializerRoundTripsPayloadAsJson() {
            var payload = JsonSerializer.SerializeToElement(new { text = "hi" });
            var original = MessageEnvelope<object>.Create("jobs", payload);

            var copy = EnvelopeSerializer.Deserialize(EnvelopeSerializer.Serialize(original));

            Assert.Equal(original.MessageId, copy.MessageId);
            Assert.Equal(original.CorrelationId, copy.CorrelationId);
            var element = Assert.IsType<JsonElement>(copy.Payload);
            Assert.Equal("hi", element.GetProperty("text").GetString());
        }

    }
}
