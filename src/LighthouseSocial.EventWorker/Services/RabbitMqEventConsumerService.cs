using LighthouseSocial.Domain.Events.Photo;
using LighthouseSocial.EventWorker.EventHandlers;
using LighthouseSocial.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using LighthouseSocial.EventWorker.Strategies;

namespace LighthouseSocial.EventWorker.Services;

public class RabbitMqEventConsumerService
    : BackgroundService
{
    //Changed from private to public
    public class EventMessage
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime OccuredAt { get; set; }
        public Guid AggregateId { get; set; }
        public JsonElement Data { get; set; }
    }

    private readonly ILogger<RabbitMqEventConsumerService> _logger;
    private readonly RabbitMqSettings _settings;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly EventDispatcher _dispatcher;

    public RabbitMqEventConsumerService(ILogger<RabbitMqEventConsumerService> logger, IOptions<RabbitMqSettings> settings, IServiceProvider serviceProvider, EventDispatcher dispatcher)
    {
        _logger = logger;
        _settings = settings.Value;
        _serviceProvider = serviceProvider;
        _dispatcher = dispatcher;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                Port = _settings.Port,
                VirtualHost = _settings.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _logger.LogInformation("RabbitMQ connection established {Host}:{Port}", _settings.HostName, _settings.Port);

            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("RabbitMQ channel created");

            await _channel.ExchangeDeclareAsync(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Topic,
                durable: _settings.Durable,
                autoDelete: _settings.AutoDelete,
                cancellationToken: cancellationToken);

            _logger.LogInformation("RabbitMQ exchange declared: {ExchangeName}", _settings.ExchangeName);

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: _settings.Durable,
                exclusive: false,
                autoDelete: _settings.AutoDelete,
                cancellationToken: cancellationToken);

            _logger.LogInformation("RabbitMQ queue declared: {QueueName}", _settings.QueueName);
            await _channel.QueueBindAsync(
                queue: _settings.QueueName,
                exchange: _settings.ExchangeName,
                routingKey: "lighthouse.photo.*",
                cancellationToken: cancellationToken);

            _logger.LogInformation("RabbitMQ queue bound to exchange with routing key pattern: lighthouse.photo.*");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing RabbitMQ");
            throw;
        }
    }

    /*private async Task HandlePhotoUploadedEventAsync(EventMessage eventMessage, CancellationToken cancellationToken)
    {
        try
        {
            var dataElement = eventMessage.Data;

            var fileName = dataElement.GetProperty("fileName").GetString() ?? string.Empty;
            var userId = dataElement.GetProperty("userId").GetGuid();
            var lighthouseId = dataElement.GetProperty("lighthouseId").GetGuid();
            var cameraType = dataElement.GetProperty("cameraType").GetString() ?? string.Empty;
            var resolution = dataElement.GetProperty("resolution").GetString() ?? string.Empty;
            var lens = dataElement.GetProperty("lens").GetString() ?? string.Empty;
            var uploadedAt = dataElement.GetProperty("uploadedAt").GetDateTime();

            var photoUploadedEvent = new PhotoUploaded(
                photoId: eventMessage.AggregateId,
                fileName: fileName,
                userId: userId,
                lighthouseId: lighthouseId,
                cameraType: cameraType,
                resolution: resolution,
                lens: lens,
                uploadedAt: uploadedAt);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IPhotoUploadedEventHander>();
            await handler.HandleAsync(photoUploadedEvent, cancellationToken);

            _logger.LogInformation("Processed PhotoUploaded event. EventId: {EventId} PhotoId: {PhotoId}", eventMessage.EventId, eventMessage.AggregateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling PhotoUploaded event. EventId: {EventId}", eventMessage.EventId);
        }
    }*/

    private async Task ProcessEventAsync(string message, string routingKey, CancellationToken cancellationToken)
    {
        try
        {
            var eventMessage = JsonSerializer.Deserialize<EventMessage>(message, _jsonSerializerOptions);
            if (eventMessage == null)
            {
                _logger.LogWarning("Failed to deserialize event message. RoutingKey: {RoutingKey}", routingKey);
                return;
            }

            //todo@buraksenyurt handle different event types without switch case
            /*switch (eventMessage.EventType)
            {
                case "PhotoUploaded":
                    await HandlePhotoUploadedEventAsync(eventMessage, cancellationToken);
                    break;
                case "LighthouseCreated":
                    // Implement LighthouseCreated event handling here
                    _logger.LogInformation("LighthouseCreated event received. EventId: {EventId}", eventMessage.EventId);
                    break;
                default:
                    _logger.LogWarning("Unhandled event type: {EventType}. EventId: {EventId}", eventMessage.EventType, eventMessage.EventId);
                    break;
            }*/

            await _dispatcher.DispatchAsync(eventMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event message. RoutingKey: {RoutingKey}", routingKey);
        }

    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cancellation requested before starting RabbitMqEventConsumerService.");
            return;
        }

        await InitializeRabbitMqAsync(stoppingToken);

        if (_channel == null)
        {
            _logger.LogError("RabbitMQ channel is not initialized.");
            return;
        }

        _logger.LogInformation("RabbitMqEventConsumerService has started.");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                _logger.LogDebug("Received message with RoutingKey: {RoutingKey}, Body: {Body}", routingKey, message);
                await ProcessEventAsync(message, routingKey, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in consumer received event");
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _settings.QueueName,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("RabbitMqEventConsumerService is consuming messages from queue: {QueueName}", _settings.QueueName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMqEventConsumerService is stopping.");

        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);

        _logger.LogInformation("RabbitMqEventConsumerService has stopped.");
    }
}