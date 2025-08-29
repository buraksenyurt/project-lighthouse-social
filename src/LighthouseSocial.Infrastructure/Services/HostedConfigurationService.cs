using LighthouseSocial.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Infrastructure.Services;

public class HostedConfigurationService
    : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HostedConfigurationService> _logger;

    public HostedConfigurationService(IServiceProvider serviceProvider, ILogger<HostedConfigurationService> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("HostedConfigurationService is starting.");
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var configService = scope.ServiceProvider.GetRequiredService<CachedConfigurationService>();

            if (configService != null)
            {
                await configService.WarmUpCacheAsync();
                _logger.LogInformation("HostedConfigurationService has completed cache warm-up.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while starting HostedConfigurationService.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("HostedConfigurationService is stopping.");
        return Task.CompletedTask;
    }
}
