using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using Microsoft.Extensions.Logging;

namespace TerminalApp.UseCases;

public class Composition(
    ILighthouseService lighthouseService,
    IPhotoService photoService,
    ILogger<Composition> logger)
{
    private readonly ILighthouseService _lighthouseService = lighthouseService;
    private readonly IPhotoService _photoService = photoService;
    private readonly ILogger<Composition> _logger = logger;

    public async Task ExecuteFullWorkflowAsync()
    {
        Console.WriteLine("Lighthouse Social Workflow Test\n");

        try
        {
            var lighthouseId = await CreateLighthouseAsync();
            if (lighthouseId == Guid.Empty)
                return;

            var photoId = await UploadPhotoAsync(lighthouseId);
            if (photoId == Guid.Empty)
                return;

            await AddCommentsToPhotoAsync(photoId);
            await ListLighthouseDetailsAsync(lighthouseId);
            await UpdateLighthouseAsync(lighthouseId);

            Console.WriteLine("\nFull workflow completed successfully!");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in comprehensive flow");
            Console.WriteLine($"Workflow failed: {ex.Message}");
        }
    }

    private async Task<Guid> CreateLighthouseAsync()
    {
        Console.WriteLine("Step 1: Creating a new lighthouse");
        Console.WriteLine("---------------------------------");

        try
        {
            var lighthouseId = Guid.NewGuid();
            var lighthouse = new LighthouseDto(
                Id: lighthouseId,
                Name: "Farol da Guia",
                CountryId: 42,
                Latitude: 38.6973,
                Longitude: -9.4187
            );

            await _lighthouseService.DeleteAsync(lighthouseId);

            var result = await _lighthouseService.CreateAsync(lighthouse);
            Console.WriteLine($"Lighthouse created: {lighthouse.Name}");
            Console.WriteLine($"\tLocation: {lighthouse.Latitude}, {lighthouse.Longitude}");
            Console.WriteLine($"\tID: {result.Data}");

            return result.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create lighthouse: {ex.Message}");
            return Guid.Empty;
        }
    }

    private async Task<Guid> UploadPhotoAsync(Guid lighthouseId)
    {
        Console.WriteLine("Step 2: Uploading a photo");
        Console.WriteLine("-------------------------");

        try
        {
            var photoPath = Path.Combine(Environment.CurrentDirectory, Constants.SamplePhotoFileName);

            if (!File.Exists(photoPath))
            {
                Console.WriteLine($"Photo file not found: {Constants.SamplePhotoFileName}");
                Console.WriteLine("\tCreating a mock photo upload...");

                await File.WriteAllTextAsync(photoPath, "Mock photo content for testing");
            }

            using var file = File.OpenRead(photoPath);

            var photoDto = new PhotoDto(
                Id: Guid.NewGuid(),
                FileName: Constants.SamplePhotoFileName,
                UploadedAt: DateTime.UtcNow.AddDays(-3),
                CameraType: "DSLR",
                UserId: Guid.NewGuid(),
                LighthouseId: lighthouseId,
                Resolution: "20MP",
                Lens: "RF 24-105mm f/4L IS USM"
            );

            var result = await _photoService.UploadAsync(photoDto, file);

            Console.WriteLine($"Photo uploaded successfully");
            Console.WriteLine($"\tPhoto ID: {result.Data}");
            Console.WriteLine($"\tFilename: {photoDto.FileName}");
            Console.WriteLine($"\tCamera: {photoDto.CameraType}");

            return result.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to upload photo: {ex.Message}");
            return Guid.Empty;
        }
    }

    private static async Task AddCommentsToPhotoAsync(Guid photoId)
    {
        Console.WriteLine("Step 3: Adding comments to photo");
        Console.WriteLine("--------------------------------");

        try
        {
            Console.WriteLine($"Photo ID: {photoId}");
            //todo@buraksenyurt Yorum ekleme işlemi için gerekli kodlar burada yer alacak.
            Console.WriteLine("\tComment functionality would be implemented here:");
            Console.WriteLine("\t\tAdd comment: 'Beautiful lighthouse at sunset!'");
            Console.WriteLine("\t\tAdd rating: 5 stars");
            Console.WriteLine("\t\tAdd comment: 'Great architectural details captured!'");
            Console.WriteLine("\t\tAdd rating: 4 stars");
            Console.WriteLine();
            Console.WriteLine("\tComments simulation completed");

            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add comments: {ex.Message}");
        }
    }

    private async Task ListLighthouseDetailsAsync(Guid lighthouseId)
    {
        Console.WriteLine("Step 4: Listing lighthouse with details");
        Console.WriteLine("---------------------------------------");

        try
        {
            var result = await _lighthouseService.GetByIdAsync(lighthouseId);

            if (result.Success)
            {
                var lighthouse = result.Data;
                if (lighthouse is null)
                {
                    Console.WriteLine($"\t{Messages.Errors.Lighthouse.LighthouseNotFound}");
                    return;
                }
                Console.WriteLine($"Lighthouse: {lighthouse.Name}");
                Console.WriteLine($"\t\tLocation: {lighthouse.Latitude}, {lighthouse.Longitude}");
                Console.WriteLine($"\t\tCountry ID: {lighthouse.CountryId}");
                Console.WriteLine($"\t\tID: {lighthouse.Id}");
                Console.WriteLine();
                Console.WriteLine("\t\tAssociated Photos:");
                Console.WriteLine($"\t\t{Constants.SamplePhotoFileName} (uploaded in this session)");
                Console.WriteLine("\t\tComments: 2 comments, avg rating: 4.5 stars");
                Console.WriteLine();
                Console.WriteLine("Lighthouse details retrieved successfully");
            }
            else
            {
                Console.WriteLine($"\t{Messages.Errors.Lighthouse.LighthouseNotFound}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\tFailed to retrieve lighthouse details: {ex.Message}");
        }
    }

    private static async Task UpdateLighthouseAsync(Guid lighthouseId)
    {
        Console.WriteLine("Step 5: Updating lighthouse information");
        Console.WriteLine("---------------------------------------");

        try
        {
            Console.WriteLine($"Lighthouse ID: {lighthouseId}");
            Console.WriteLine("\tUpdate functionality would be implemented here:");
            Console.WriteLine("\t\tUpdate description: 'Historic lighthouse with stunning ocean views'");
            Console.WriteLine("\t\tUpdate accessibility info: 'Open to public, guided tours available'");
            Console.WriteLine("\t\tUpdate status: 'Active and operational'");
            Console.WriteLine();
            Console.WriteLine("\tLighthouse update simulation completed");

            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{Messages.Errors.Lighthouse.FailedToUpdateLighthouse}: {ex.Message}");
        }
    }
}
