using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using Microsoft.Extensions.Logging;

namespace TerminalApp.UseCases;

public class PhotoManagementUseCase(
    IPhotoService photoService,
    ILighthouseService lighthouseService,
    ILogger<PhotoManagementUseCase> logger)
{
    private readonly IPhotoService _photoService = photoService;
    private readonly ILighthouseService _lighthouseService = lighthouseService;
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

    public async Task ListPhotosForLighthouseAsync(Guid lighthouseId)
    {
        Console.WriteLine("Testing: Photos of Lighthouse\n");
        Console.WriteLine($"Listing photos for Lighthouse ID: {lighthouseId}");
        try
        {
            var result = await _lighthouseService.GetPhotosByIdAsync(lighthouseId);
            if (!result.Success || result.Data is null)
            {
                Console.WriteLine($"Error retrieving photos: {result.ErrorMessage}");
                return;
            }
            foreach (var photo in result.Data)
            {
                Console.WriteLine($"Photo ID: {photo.Id}");
                Console.WriteLine($"Filename: {photo.FileName}");
                Console.WriteLine($"Uploaded At: {photo.UploadedAt:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Camera Type: {photo.CameraType}");
                Console.WriteLine($"Resolution: {photo.Resolution}");
                Console.WriteLine($"Lens: {photo.Lens}");
                Console.WriteLine(new string('-', 40));
                var response = await _photoService.GetRawPhotoAsync(photo.FileName);
                if (!response.Success || response.Data is null)
                {
                    Console.WriteLine($"Error retrieving photo stream for {photo.FileName}: {response.ErrorMessage}");
                    continue;
                }

                if (response.Data is not MemoryStream memoryStream)
                {
                    _logger.LogWarning("Failed to cast stream to MemoryStream for {Filename}", photo.FileName);
                    Console.WriteLine($"Failed to retrieve photo stream for {photo.FileName}");
                    continue;
                }
                else
                {
                    var directory = Path.Combine(Environment.CurrentDirectory, "downloads");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var filePath = Path.Combine(Environment.CurrentDirectory, "downloads", photo.FileName);
                    await File.WriteAllBytesAsync(filePath, memoryStream.ToArray());
                    Console.WriteLine($"Photo saved to: {filePath}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing photos for lighthouse");
            Console.WriteLine($"Error listing photos: {ex.Message}");
        }
    }
}
