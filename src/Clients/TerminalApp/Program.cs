using LighthouseSocial.Application;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.AddLogging(builder => builder.AddConsole());

var connStr = config.GetConnectionString("LighthouseDb");
services.AddDatabase(connStr);
services.AddApplication();

var serviceProvider = services.BuildServiceProvider();
var lighthouseService = serviceProvider.GetRequiredService<ILighthouseService>();

try
{

    var newLighthouse = new LighthouseDto(Id: Guid.NewGuid(),
        Name: "Test Lighthouse",
        CountryId: 90,
        Latitude: 37.7749,
        Longitude: -122.4194);

    var newId = await lighthouseService.CreateAsync(newLighthouse);
    Console.WriteLine($"Created new lighthouse with ID: {newId}");

    var allLighthouses = await lighthouseService.GetAllAsync();
    Console.WriteLine("All Lighthouses:");
    foreach (var lighthouse in allLighthouses)
    {
        Console.WriteLine($"- {lighthouse.Name} (ID: {lighthouse.Id})");
    }
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while processing lighthouses.");
}
finally
{
    if (serviceProvider is IDisposable disposable)
    {
        disposable.Dispose();
    }
}