using LighthouseSocial.Application;
using LighthouseSocial.Data;
using LighthouseSocial.Infrastructure;
using LighthouseSocial.Infrastructure.Configuration;
using LighthouseSocial.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDatabase(provider =>
{
    var vaultConfigService = provider.GetRequiredService<VaultConfigurationService>();
    try
    {
        return vaultConfigService.GetDatabaseConnectionStringAsync().GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Failed to get connection string from Vault");
        return string.Empty;
    }
});
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("Minio"));

// builder.Services.AddOpenApi();

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
