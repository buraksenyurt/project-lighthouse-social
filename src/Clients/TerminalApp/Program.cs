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
//todo@buraksenyurt Connection string bilgisini güvenli bir şekilde saklamalıyız. Secure Vault, Azure Key Vault gibi çözümler kullanılabilir.
services.AddDatabase(connStr ?? "Host=localhost;Port=5432;Database=lighthousedb;Username=johndoe;Password=somew0rds");
services.AddApplication();

var serviceProvider = services.BuildServiceProvider();
var lighthouseService = serviceProvider.GetRequiredService<ILighthouseService>();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();


try
{
    var id = Guid.Parse("4C2D0945-53D3-459B-B523-AD6A8B212554");

    await lighthouseService.DeleteAsync(id); //todo@buraksenyurt Geriye birşey döndürmüyor. Nasıl değerlendirilebilir?

    var newLighthouse = new LighthouseDto(
        Id: id,
        Name: "Cape Espichel",
        CountryId: 42, // Portekiz
        Latitude: 38.533,
        Longitude: 9.12);

    var addedId = await lighthouseService.CreateAsync(newLighthouse);
    Console.WriteLine($"Lighthouse created with ID: {addedId}");

    var lighthouse = await lighthouseService.GetByIdAsync(id);
    if (lighthouse is not null)
    {
        Console.WriteLine($"Lighthouse found: {lighthouse.Name} (ID: {lighthouse.Id}) in {lighthouse.CountryId}");
    }
    else
    {
        logger.LogWarning("Lighthouse {lighthouse.Id} not found.", id);
    }

    var allLighthouses = await lighthouseService.GetAllAsync();
    Console.WriteLine("All Lighthouses:");
    foreach (var l in allLighthouses)
    {
        Console.WriteLine($"- {l.Name} (ID: {l.Id})");
    }

    //todo@buraksenyurt Tam bir flow test edelim

    // Deniz feneri bilgisi ekle
    // Deniz fenerine fotoğraf ekle
    // Deniz feneri fotoğrafına birkaç yorum ekle
    // Fotoğraf ID'sine göre deniz feneri ve yorumları listele
    // Deniz feneri bilgisinde güncelleme yap
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while processing lighthouses.");
}
finally
{
    if (serviceProvider is IDisposable disposable)
    {
        disposable.Dispose();
    }
}