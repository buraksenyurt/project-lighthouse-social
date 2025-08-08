using LighthouseSocial.Application.Contracts;
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
            var countries = await countryDataReader.GetAllAsync();
            foreach (var country in countries)
            {
                Console.WriteLine($"{country.Id}:{country.Name}");
            }

            var portugal = await countryDataReader.GetByIdAsync(42);
            Console.WriteLine($"{portugal.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on test");
            Console.WriteLine(ex.Message);
        }
    }
}
