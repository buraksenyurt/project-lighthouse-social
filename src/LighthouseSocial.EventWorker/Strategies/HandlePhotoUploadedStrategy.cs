using LighthouseSocial.Domain.Events.Photo;
using LighthouseSocial.EventWorker.EventHandlers;
using LighthouseSocial.EventWorker.Services;

namespace LighthouseSocial.EventWorker.Strategies;

public class HandlePhotoUploadedStrategy : IEventStrategy
{
    public string EventType { get; } = "PhotoUploaded";

    private readonly ILogger<HandlePhotoUploadedStrategy> _logger;
    private readonly IServiceProvider _serviceProvider;

    public HandlePhotoUploadedStrategy(ILogger<HandlePhotoUploadedStrategy> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleEventAsync(RabbitMqEventConsumerService.EventMessage eventMessage, CancellationToken cancellationToken)
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
    }
}