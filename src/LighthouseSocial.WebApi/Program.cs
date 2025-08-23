using LighthouseSocial.Application;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Data;
using LighthouseSocial.Infrastructure;
using LighthouseSocial.Infrastructure.Auditors;
using LighthouseSocial.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services
    .AddInfrastructure(builder.Configuration)
    .WithSecretVault()
    .WithStorage()
    .WithCaching()
    .WithExternals()
    .Build();

builder.Services.AddData();
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("Minio"));
builder.Services.AddHttpClient<ICommentAuditor, ExternalCommentAuditor>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
