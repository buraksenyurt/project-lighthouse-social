using LighthouseSocial.Domain.Common;
using LighthouseSocial.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace LighthouseSocial.Infrastructure.Messaging;

public class RabbitMqEventPublisher
    : IEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly ConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _exchangeDeclared;
    private readonly SemaphoreSlim _initializationSemaphore = new(1, 1);
    private bool _disposed;

    public RabbitMqEventPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqEventPublisher> logger)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _connectionFactory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost,
        };

        _logger.LogInformation("RabbitMQ Event Publisher initialized with Host: {Host}, Port: {Port}, VirtualHost: {VirtualHost}, Exchange: {Exchange}",
            _settings.HostName, _settings.Port, _settings.VirtualHost, _settings.ExchangeName);
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        if (_connection != null && _channel != null && _exchangeDeclared)
            return;

        await _initializationSemaphore.WaitAsync(cancellationToken);

        try
        {
            if (_connection != null && _channel != null && _exchangeDeclared)
                return;

            if (_connection == null)
            {
                _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
                _logger.LogDebug("RabbitMQ connection established.");
            }

            if (_channel == null)
            {
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                _logger.LogDebug("RabbitMQ channel created.");
            }

            if (!_exchangeDeclared)
            {
                await _channel.ExchangeDeclareAsync(
                    exchange: _settings.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: _settings.Durable,
                    autoDelete: _settings.AutoDelete,
                    cancellationToken: cancellationToken);

                _exchangeDeclared = true;
                _logger.LogDebug("RabbitMQ exchange '{Exchange}' declared.", _settings.ExchangeName);
            }
        }
        finally
        {
            _initializationSemaphore.Release();
        }

        await Task.CompletedTask;
    }

    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        await EnsureInitializedAsync(cancellationToken);

        try
        {

            var eventMessage = new EventMessage
            {
                EventId = @event.EventId,
                EventType = @event.EventType,
                OccurredAt = @event.OccuredAt,
                AggregateId = @event.AggregateId,
                Data = @event
            };

            var json = JsonSerializer.Serialize(eventMessage, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
            var body = Encoding.UTF8.GetBytes(json);
            var routingKey = $"lighthouse.{@event.EventType.ToLowerInvariant()}";

            var basitProperties = new BasicProperties
            {
                Persistent = true,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel!.BasicPublishAsync(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: basitProperties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Published event {EventType} with ID {EventId} to exchange {Exchange} with routing key {RoutingKey}",
                @event.EventType, @event.EventId, _settings.ExchangeName, routingKey);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType} with ID {EventId}", @event.EventType, @event.EventId);
            throw;
        }
    }

    public async Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(events);

        foreach (var @event in events)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _initializationSemaphore?.Dispose();

            if (_channel != null)
            {
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }

            _disposed = true;
            _logger.LogInformation("RabbitMQ Event Publisher disposed.");
        }

        GC.SuppressFinalize(this);
    }

    public void Dipose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    private class EventMessage
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public Guid AggregateId { get; set; }
        public object Data { get; set; } = null!;
    }
}
