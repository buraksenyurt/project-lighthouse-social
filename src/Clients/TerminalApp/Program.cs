using LighthouseSocial.Application;
using LighthouseSocial.Data;
using LighthouseSocial.Infrastructure;
using LighthouseSocial.Infrastructure.Configuration;
using LighthouseSocial.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TerminalApp.UseCases;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.AddLogging(builder => builder.AddConsole());

services.AddApplication();
services.AddInfrastructure(config);
services.AddData();
services.Configure<MinioSettings>(config.GetSection("Minio"));

services.AddScoped<LighthouseManagement>();
services.AddScoped<PhotoManagementUseCase>();
services.AddScoped<Composition>();
services.AddScoped<ViewManagement>();
services.AddSingleton<IConfiguration>(config);

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("LighthouseSocial Terminal Client started");

await RunInteractiveCliAsync(serviceProvider, logger);

async Task RunInteractiveCliAsync(ServiceProvider serviceProvider, ILogger<Program> logger)
{
    try
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("LighthouseSocial Terminal Client\n");
            Console.WriteLine("Available Use Cases:");
            Console.WriteLine("1.Lighthouse Management (CRUD Operations)");
            Console.WriteLine("2.Photo Management (Upload & Operations)");
            Console.WriteLine("3.Composition Flow Test (Full Workflow)");
            Console.WriteLine("4.Cached Countries Test");
            Console.WriteLine("5.Photo List of Lighthouse Test");
            Console.WriteLine("6.Exit");
            Console.WriteLine();
            Console.Write("Select an option (1-6): ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ExecuteLighthouseManagementAsync(serviceProvider);
                    break;
                case "2":
                    await ExecutePhotoManagementAsync(serviceProvider);
                    break;
                case "3":
                    await ExecuteComprehensiveFlowAsync(serviceProvider);
                    break;
                case "4":
                    await ExecuteCountryCacheTestAsync(serviceProvider);
                    break;
                case "5":
                    await ExecuteListPhotosForLighthouseTestAsync(serviceProvider);
                    break;
                case "6":
                    Console.WriteLine("See you later, elegaytır :D");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }

            if (choice != "6")
            {
                Console.WriteLine("\nPress any key to return to main menu...");
                Console.ReadKey();
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred in the CLI");
        Console.WriteLine($"Application error: {ex.Message}");
    }
    finally
    {
        if (serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

async Task ExecuteLighthouseManagementAsync(ServiceProvider serviceProvider)
{
    Console.Clear();
    Console.WriteLine("Lighthouse Management Use Case\n");

    var useCase = serviceProvider.GetRequiredService<LighthouseManagement>();
    await useCase.CreateAndTestLighthouseAsync();
}

async Task ExecutePhotoManagementAsync(ServiceProvider serviceProvider)
{
    Console.Clear();
    Console.WriteLine("Photo Management Use Case\n");

    var useCase = serviceProvider.GetRequiredService<LighthouseManagement>();
    var lighthouseId = await useCase.GetOrCreateSampleLighthouseAsync();

    var photoUseCase = serviceProvider.GetRequiredService<PhotoManagementUseCase>();
    await photoUseCase.UploadLighthousePhotoAsync(lighthouseId);
}

async Task ExecuteComprehensiveFlowAsync(ServiceProvider serviceProvider)
{
    Console.Clear();
    Console.WriteLine("Composition Flow Test Use Case\n");

    var useCase = serviceProvider.GetRequiredService<Composition>();
    await useCase.ExecuteFullWorkflowAsync();
}

async Task ExecuteCountryCacheTestAsync(ServiceProvider serviceProvider)
{
    Console.Clear();
    Console.WriteLine("Country Cache Test Use Case\n");

    var useCase = serviceProvider.GetRequiredService<ViewManagement>();
    await useCase.LoadCountryList();
}

async Task ExecuteListPhotosForLighthouseTestAsync(ServiceProvider serviceProvider)
{
    Console.Clear();
    Console.WriteLine("List Photos of Lighthouse Test\n");
    var useCase = serviceProvider.GetRequiredService<PhotoManagementUseCase>();
    await useCase.ListPhotosForLighthouseAsync(Guid.Parse(Constants.SampleLighthouseId));
}