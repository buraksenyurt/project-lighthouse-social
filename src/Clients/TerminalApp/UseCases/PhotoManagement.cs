using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using Microsoft.Extensions.Logging;

namespace TerminalApp.UseCases;

public class PhotoManagementUseCase(
    IPhotoService photoService,
    ILogger<PhotoManagementUseCase> logger)
{
    private readonly IPhotoService _photoService = photoService;
    private readonly ILogger<PhotoManagementUseCase> _logger = logger;

    public async Task UploadLighthousePhotoAsync(Guid lighthouseId)
    {
        Console.WriteLine("Testing Photo Upload\n");

        try
        {
            var photoPath = Path.Combine(Environment.CurrentDirectory, Constants.SamplePhotoFileName);

            if (!File.Exists(photoPath))
            {
                Console.WriteLine($"Photo file not found: {Constants.SamplePhotoFileName}");
                Console.WriteLine("Please ensure 'cape_espichel.png' exists in the application directory.");
                return;
            }

            Console.WriteLine($"Opening photo file: {Constants.SamplePhotoFileName}");
            using var file = File.OpenRead(photoPath);

            var photoDto = new PhotoDto(
                Id: Guid.NewGuid(),
                FileName: Constants.SamplePhotoFileName,
                UploadedAt: DateTime.UtcNow.AddDays(-7),
                CameraType: "DSLR",
                UserId: Guid.NewGuid(),
                LighthouseId: lighthouseId,
                Resolution: "45MP",
                Lens: "RF 24-70mm f/2.8L IS USM"
            );

            Console.WriteLine("Uploading photo...");
            var uploadedPhotoId = await _photoService.UploadAsync(photoDto, file);

            Console.WriteLine($"Photo uploaded successfully!");
            Console.WriteLine($"Photo ID: {uploadedPhotoId}");
            Console.WriteLine($"Lighthouse ID: {lighthouseId}");
            Console.WriteLine($"Filename: {Constants.SamplePhotoFileName}");
            Console.WriteLine($"Upload Date: {photoDto.UploadedAt:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Camera: {photoDto.CameraType}");
            Console.WriteLine($"Lens: {photoDto.Lens}");
            Console.WriteLine($"Resolution: {photoDto.Resolution}");

        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Photo file 'cape_espichel.png' not found in application directory.");
            Console.WriteLine("Please add a sample photo file to test the upload functionality.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during photo upload");
            Console.WriteLine($"Error uploading photo: {ex.Message}");
        }

        Console.WriteLine("\nPhoto upload test completed!");
    }

    public Task ListPhotosForLighthouseAsync(Guid lighthouseId)
    {
        throw new NotImplementedException();
    }
}
