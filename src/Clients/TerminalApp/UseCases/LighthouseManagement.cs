using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using Microsoft.Extensions.Logging;

namespace TerminalApp.UseCases;

public class LighthouseManagement(
    ILighthouseService lighthouseService,
    ILogger<LighthouseManagement> logger)
{
    private readonly ILighthouseService _lighthouseService = lighthouseService;
    private readonly ILogger<LighthouseManagement> _logger = logger;

    public async Task CreateAndTestLighthouseAsync()
    {
        Console.WriteLine("Lighthouse CRUD Operations\n");

        var id = Guid.Parse(Constants.SampleLighthouseId);

        try
        {
            Console.WriteLine("Cleaning up existing lighthouse...");
            await _lighthouseService.DeleteAsync(id);

            Console.WriteLine("Creating new lighthouse...");
            var newLighthouse = new LighthouseDto(
                Id: id,
                Name: "Cape Espichel",
                CountryId: 42,
                Latitude: 38.533,
                Longitude: 9.12);

            var addedId = await _lighthouseService.CreateAsync(newLighthouse);
            Console.WriteLine($"Lighthouse created with ID: {addedId}");

            Console.WriteLine("Retrieving created lighthouse...");
            var lighthouse = await _lighthouseService.GetByIdAsync(id);
            if (lighthouse is not null)
            {
                Console.WriteLine($"Lighthouse found: {lighthouse.Data.Name} (ID: {lighthouse.Data.Id}) in Country: {lighthouse.Data.CountryId}");
                Console.WriteLine($"Location: {lighthouse.Data.Latitude}, {lighthouse.Data.Longitude}");
            }
            else
            {
                _logger.LogWarning("Lighthouse {LighthouseId} not found.", id);
                Console.WriteLine("Lighthouse not found!");
            }

            Console.WriteLine("\nAll Lighthouses in System:");
            var allLighthouses = await _lighthouseService.GetPagedAsync(new PagingDto
            {
                Page = 1,
                PageSize = 100 // Adjust as needed
            });

            if (allLighthouses.Data.Items.Any())
            {
                foreach (var l in allLighthouses.Data.Items)
                {
                    Console.WriteLine($"\t{l.Name} (ID: {l.Id})");
                }
            }
            else
            {
                Console.WriteLine("\tNo lighthouses found in the system.");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in lighthouse management use case");
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nLighthouse management test completed!");
    }

    public async Task<Guid> GetOrCreateSampleLighthouseAsync()
    {
        var id = Guid.Parse(Constants.SampleLighthouseId);

        var lighthouse = await _lighthouseService.GetByIdAsync(id);
        if (lighthouse == null)
        {
            Console.WriteLine("Creating sample lighthouse for testing...");
            var newLighthouse = new LighthouseDto(
                Id: id,
                Name: "Cape Espichel",
                CountryId: 42,
                Latitude: 38.533,
                Longitude: 9.12);

            await _lighthouseService.CreateAsync(newLighthouse);
            Console.WriteLine("Sample lighthouse created!");
        }

        return id;
    }
}
