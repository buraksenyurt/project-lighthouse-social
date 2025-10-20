using LighthouseSocial.Domain.Events.Photo;
using LighthouseSocial.EventWorker.EventHandlers;
using LighthouseSocial.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace LighthouseSocial.EventWorker.Services;

public class RabbitMqEventConsumerService
    : BackgroundService
{
    private class EventMessage
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime OccuredAt { get; set; }
        public Guid AggregateId { get; set; }
        public Object Payload { get; set; } = new();
    }

    private readonly ILogger<RabbitMqEventConsumerService> _logger;
    private readonly RabbitMqSettings _settings;
    private readonly IServiceProvider serviceProvider;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqEventConsumerService(ILogger<RabbitMqEventConsumerService> logger, IOptions<RabbitMqSettings> settings, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _settings = settings.Value;
        this.serviceProvider = serviceProvider;
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

    private async Task HandlePhotoUploadedEventAsync(EventMessage eventMessage, CancellationToken cancellationToken)
    {
        var photoUploadedEvent = JsonSerializer.Deserialize<PhotoUploaded>(eventMessage.Payload.ToString() ?? string.Empty);

        if (photoUploadedEvent == null)
        {
            _logger.LogWarning("Failed to deserialize PhotoUploaded event payload. EventId: {EventId}", eventMessage.EventId);
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IPhotoUploadedEventHander>();
        await handler.HandleAsync(photoUploadedEvent, cancellationToken);

        _logger.LogInformation("Processed PhotoUploaded event. EventId: {EventId} PhotoId: {PhotoId}", eventMessage.EventId, eventMessage.AggregateId);
    }

    private async Task ProcessEventAsync(string message, string routingKey, CancellationToken cancellationToken)
    {
        try
        {
            var eventMessage = JsonSerializer.Deserialize<EventMessage>(message);
            if (eventMessage == null)
            {
                _logger.LogWarning("Failed to deserialize event message. RoutingKey: {RoutingKey}", routingKey);
                return;
            }

            //todo@buraksenyurt handle different event types without switch case
            switch (eventMessage.EventType)
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
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event message. RoutingKey: {RoutingKey}", routingKey);
        }

    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMqEventConsumerService is starting.");

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