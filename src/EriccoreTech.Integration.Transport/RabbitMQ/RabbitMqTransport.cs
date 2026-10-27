using System.Runtime.CompilerServices;
using System.Threading.Channels;
using EriccoreTech.Integration.Contracts;
using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EriccoreTech.Integration.Transport {

    // One fanout exchange per topic; every subscriber gets its own exclusive, auto-deleted queue.
    public sealed class RabbitMqTransport : IMessageTransport, IAsyncDisposable {

        private readonly ConnectionFactory _factory;
        private readonly SemaphoreSlim _gate = new(1, 1);
        private IConnection? _connection;

        public RabbitMqTransport(RabbitMqOptions options) {
            _factory = new ConnectionFactory {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost
            };
        }

        public async Task PublishAsync(string topic, MessageEnvelope<object> message, CancellationToken cancellationToken = default) {
            if (message.IsExpired(DateTimeOffset.UtcNow)) {
                return;
            }

            var connection = await GetConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await channel.ExchangeDeclareAsync(topic, ExchangeType.Fanout, durable: true, cancellationToken: cancellationToken);
            await channel.BasicPublishAsync(topic, string.Empty, EnvelopeSerializer.Serialize(message), cancellationToken);
        }

        public async IAsyncEnumerable<MessageEnvelope<object>> Subscribe(
            string topic, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
            var connection = await GetConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await channel.ExchangeDeclareAsync(topic, ExchangeType.Fanout, durable: true, cancellationToken: cancellationToken);
            var queue = await channel.QueueDeclareAsync(cancellationToken: cancellationToken);
            await channel.QueueBindAsync(queue.QueueName, topic, string.Empty, cancellationToken: cancellationToken);

            var received = Channel.CreateUnbounded<MessageEnvelope<object>>();
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, args) => {
                var message = EnvelopeSerializer.Deserialize(args.Body.Span);
                await received.Writer.WriteAsync(message, cancellationToken);
                await channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
            };
            await channel.BasicConsumeAsync(queue.QueueName, false, consumer, cancellationToken);

            await foreach (var message in received.Reader.ReadAllAsync(cancellationToken)) {
                if (!message.IsExpired(DateTimeOffset.UtcNow)) {
                    yield return message;
                }
            }
        }

        public async ValueTask DisposeAsync() {
            if (_connection is not null) {
                await _connection.DisposeAsync();
            }

            _gate.Dispose();
        }

        private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken) {
            if (_connection is { IsOpen: true }) {
                return _connection;
            }

            await _gate.WaitAsync(cancellationToken);
            try {
                if (_connection is not { IsOpen: true }) {
                    _connection = await _factory.CreateConnectionAsync(cancellationToken);
                }

                return _connection;
            } finally {
                _gate.Release();
            }
        }

    }
}
