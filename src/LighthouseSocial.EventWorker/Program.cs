using LighthouseSocial.EventWorker.EventHandlers;
using LighthouseSocial.EventWorker.Services;
using LighthouseSocial.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .WithCaching()
    .WithSecretVault()
    .WithGraylog(builder.Environment)
    .WithMessaging()
    .Build();

builder.Services.AddScoped<IPhotoUploadedEventHander, PhotoUploadedEventHandler>();

builder.Services.AddHostedService<RabbitMqEventConsumerService>();

var host = builder.Build();
await host.RunAsync();
