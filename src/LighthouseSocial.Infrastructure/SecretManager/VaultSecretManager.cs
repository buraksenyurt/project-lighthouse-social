using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace LighthouseSocial.Infrastructure.SecretManager;

public class VaultSecretManager
    : ISecretManager
{
    private readonly VaultSettings _settings;
    private readonly ILogger<VaultSecretManager> _logger;
    private readonly IVaultClient _vaultClient;
    public VaultSecretManager(IConfiguration configuration, ILogger<VaultSecretManager> logger)
    {
        _logger = logger;
        _settings = new VaultSettings();
        configuration.GetSection("VaultSettings").Bind(_settings);

        _vaultClient = new VaultClient(new VaultClientSettings(_settings.Address, new TokenAuthMethodInfo(_settings.Token)));

        _logger.LogInformation("VaultSecretManager initialized with Address: {Address}, MountPoint: {MountPoint}",
            _settings.Address, _settings.MountPoint);
    }
    public async Task<string?> GetSecretAsync(string secretPath, string key)
    {
        try
        {
            _logger.LogInformation("Retrieving secret from Vault at path: {SecretPath}, key: {Key}", secretPath, key);

            var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: secretPath, mountPoint: _settings.MountPoint);
            if (secret.Data.Data.TryGetValue(key, out var value))
            {
                _logger.LogInformation("Successfully retrieved secret for key: {Key}", key);
                return value?.ToString();
            }

            _logger.LogWarning("Key: {Key} not found in secret at path: {SecretPath}", key, secretPath);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving secret from Vault at path: {SecretPath}, key: {Key}", secretPath, key);
            return null;
        }
    }

    public async Task<Dictionary<string, string>> GetSecretsAsync(string secretPath)
    {
        try
        {
            _logger.LogInformation("Retrieving all secrets from Vault at path: {SecretPath}", secretPath);
            var result = new Dictionary<string, string>();

            var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: secretPath, mountPoint: _settings.MountPoint);
            if (secret?.Data?.Data != null)
            {
                foreach (var kvp in secret.Data.Data)
                {
                    if (kvp.Value is string value)
                    {
                        result[kvp.Key] = value;
                    }
                }
            }
            else
            {
                _logger.LogWarning("{NoSecretsFound}: {SecretPath}", Messages.Errors.SecureVault.NoSecretsFound, secretPath);
            }

            _logger.LogInformation("Successfully retrieved {Count} secrets from path: {SecretPath}", result.Count, secretPath);
            return result;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorRetrieving}: {SecretPath}", Messages.Errors.SecureVault.RetrievingSecrets, secretPath);
            return [];
        }
    }
}
