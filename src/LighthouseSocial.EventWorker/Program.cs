using LighthouseSocial.EventWorker.EventHandlers;
using LighthouseSocial.EventWorker.Services;
using LighthouseSocial.EventWorker.Strategies;
using LighthouseSocial.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration)
    .WithCaching()
    .WithSecretVault()
    .WithGraylog(builder.Environment)
    .WithMessaging()
    .Build();

builder.Services.AddScoped<IPhotoUploadedEventHandler, PhotoUploadedEventHandler>();

builder.Services.AddSingleton<EventDispatcher>();
builder.Services.AddSingleton<IEventStrategy, HandlePhotoUploadedStrategy>();

builder.Services.AddHostedService<RabbitMqEventConsumerService>();

var host = builder.Build();
await host.RunAsync();
