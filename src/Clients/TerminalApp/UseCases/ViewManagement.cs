using LighthouseSocial.Application.Contracts.Repositories;
using Microsoft.Extensions.Logging;

namespace TerminalApp.UseCases;

public class ViewManagement(ICountryDataReader countryDataReader, ILogger<ViewManagement> logger)
{
    private readonly ILogger<ViewManagement> _logger = logger;

    public async Task LoadCountryList()
    {
        Console.WriteLine("Loading countries");
        try
        {
            var countriesResult = await countryDataReader.GetAllAsync();
            if (!countriesResult.Success)
            {
                Console.WriteLine($"Failed to load countries: {countriesResult.ErrorMessage}");
                return;
            }

            foreach (var country in countriesResult.Data!)
            {
                Console.WriteLine($"{country.Id}:{country.Name}");
            }

            var result = await countryDataReader.GetByIdAsync(42);
            if (!result.Success)
            {
                Console.WriteLine(result.ErrorMessage);
                return;
            }
            Console.WriteLine($"{result.Data!.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on test");
            Console.WriteLine(ex.Message);
        }
    }
}
