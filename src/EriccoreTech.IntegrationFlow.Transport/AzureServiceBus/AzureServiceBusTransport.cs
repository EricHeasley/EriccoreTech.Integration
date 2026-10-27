using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Azure.Messaging.ServiceBus;
using EriccoreTech.IntegrationFlow.Contracts;
using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Messaging;

namespace EriccoreTech.IntegrationFlow.Transport {

    // Topics map to Service Bus topics. Publishing and subscribing need the topic and subscription provisioned.
    public sealed class AzureServiceBusTransport : IMessageTransport, IAsyncDisposable {

        private readonly ServiceBusClient _client;
        private readonly AzureServiceBusOptions _options;

        public AzureServiceBusTransport(AzureServiceBusOptions options) {
            _options = options;
            _client = new ServiceBusClient(options.ConnectionString);
        }

        public async Task PublishAsync(string topic, MessageEnvelope<object> message, CancellationToken cancellationToken = default) {
            if (message.IsExpired(DateTimeOffset.UtcNow)) {
                return;
            }

            await using var sender = _client.CreateSender(topic);
            var serviceBusMessage = new ServiceBusMessage(EnvelopeSerializer.Serialize(message)) {
                MessageId = message.MessageId.ToString(),
                CorrelationId = message.CorrelationId.ToString(),
                Subject = message.MessageType,
                ContentType = "application/json"
            };
            if (message.ExpiresAt is { } expires) {
                serviceBusMessage.TimeToLive = expires - DateTimeOffset.UtcNow;
            }

            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        }

        public async IAsyncEnumerable<MessageEnvelope<object>> Subscribe(
            string topic, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
            var received = Channel.CreateUnbounded<MessageEnvelope<object>>();
            await using var processor = _client.CreateProcessor(topic, _options.SubscriptionName, new ServiceBusProcessorOptions {
                AutoCompleteMessages = false
            });
            processor.ProcessMessageAsync += async args => {
                var message = EnvelopeSerializer.Deserialize(args.Message.Body.ToMemory().Span);
                await received.Writer.WriteAsync(message, args.CancellationToken);
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            };
            processor.ProcessErrorAsync += args => {
                // Transient errors are retried by the processor; anything else ends the subscription.
                if (args.Exception is not ServiceBusException { IsTransient: true }) {
                    received.Writer.TryComplete(args.Exception);
                }

                return Task.CompletedTask;
            };
            await processor.StartProcessingAsync(cancellationToken);

            try {
                await foreach (var message in received.Reader.ReadAllAsync(cancellationToken)) {
                    if (!message.IsExpired(DateTimeOffset.UtcNow)) {
                        yield return message;
                    }
                }
            } finally {
                await processor.StopProcessingAsync(CancellationToken.None);
            }
        }

        public async ValueTask DisposeAsync() {
            await _client.DisposeAsync();
        }

    }
}
