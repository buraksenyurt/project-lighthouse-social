using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Infrastructure.Identity;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Infrastructure.Configuration;

public class VaultConfigurationService(ISecretManager secretManager, ILogger<VaultConfigurationService> logger)
{
    private const string SecretPath = "ProjectLighthouseSocial-Dev";

    public async Task<string> GetDatabaseConnectionStringAsync()
    {
        try
        {
            var connectionString = await secretManager.GetSecretAsync(SecretPath, "DbConnStr");
            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogWarning("{ConnectionStringNotFound} at path: {SecretPath}", Messages.Errors.SecureVault.DatabaseConnectionStringNotFound, SecretPath);
                throw new InvalidOperationException(Messages.Errors.SecureVault.DatabaseConnectionStringNotFound);
            }
            return connectionString;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, Messages.Errors.SecureVault.RetrievingDbConnectionString);
            return string.Empty;
        }
    }

    public async Task<(string AccessKey, string SecretKey)> GetMinioCredentialsAsync()
    {
        try
        {
            var accessKey = await secretManager.GetSecretAsync(SecretPath, "MinIOAccessKey");
            var secretKey = await secretManager.GetSecretAsync(SecretPath, "MinIOSecretKey");

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                logger.LogWarning("{ErrorMessage} at path: {SecretPath}", Messages.Errors.SecureVault.MinioCredentialsNotFound, SecretPath);
                throw new InvalidOperationException(Messages.Errors.SecureVault.MinioCredentialsNotFound);
            }

            logger.LogInformation("Successfully retrieved MinIO credentials from Vault");
            return (AccessKey: accessKey, SecretKey: secretKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, Messages.Errors.SecureVault.RetrievingMinio);
            return (string.Empty, string.Empty);
        }
    }

    public async Task<KeycloakSettings> GetKeycloakSettingsAsync()
    {
        try
        {
            var keycloakSettings = new KeycloakSettings
            {
                Audience = await secretManager.GetSecretAsync(SecretPath, "KeycloakAudience"),
                Authority = await secretManager.GetSecretAsync(SecretPath, "KeycloakAuthority"),
                ClientId = await secretManager.GetSecretAsync(SecretPath, "KeycloakClientId"),
                ClientSecret = await secretManager.GetSecretAsync(SecretPath, "KeycloakClientSecret"),
                Realm = await secretManager.GetSecretAsync(SecretPath, "KeycloakRealm"),
                ClockSkew = int.TryParse(await secretManager.GetSecretAsync(SecretPath, "KeycloakClockSkew"), out var clockSkew) ? clockSkew : 5,
                RequireHttpsMetadata = !bool.TryParse(await secretManager.GetSecretAsync(SecretPath, "KeycloakRequireHttpsMetadata"), out var requireHttps) || requireHttps,
                ValidateAudience = !bool.TryParse(await secretManager.GetSecretAsync(SecretPath, "KeycloakValidateAudience"), out var validateAudience) || validateAudience,
                ValidateIssuer = !bool.TryParse(await secretManager.GetSecretAsync(SecretPath, "KeycloakValidateIssuer"), out var validateIssuer) || validateIssuer,
                ValidateIssuerSigningKey = !bool.TryParse(await secretManager.GetSecretAsync(SecretPath, "KeycloakValidateIssuerSigningKey"), out var validateIssuerSigningKey) || validateIssuerSigningKey,
                ValidateLifetime = !bool.TryParse(await secretManager.GetSecretAsync(SecretPath, "KeycloakValidateLifetime"), out var validateLifetime) || validateLifetime
            };
            if(string.IsNullOrEmpty(keycloakSettings.Audience) ||
               string.IsNullOrEmpty(keycloakSettings.Authority) ||
               string.IsNullOrEmpty(keycloakSettings.ClientId) ||
               string.IsNullOrEmpty(keycloakSettings.ClientSecret) ||
               string.IsNullOrEmpty(keycloakSettings.Realm))
            {
                logger.LogWarning("{ErrorMessage} at path: {SecretPath}", Messages.Errors.SecureVault.RetrievingKeycloak, SecretPath);
                throw new InvalidOperationException(Messages.Errors.SecureVault.RetrievingKeycloak);
            }
            logger.LogInformation("Successfully retrieved Keycloak settings from Vault");
            return keycloakSettings;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error retrieving Keycloak settings from Vault");
            return new KeycloakSettings();
        }
    }

    public async Task<Dictionary<string, string>> GetAllSecretsAsync()
    {
        try
        {
            var secrets = await secretManager.GetSecretsAsync(SecretPath);
            logger.LogInformation("Retrieved {Count} secrets from Vault at path: {SecretPath}", secrets.Count, SecretPath);
            return secrets;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all secrets from Vault");
            return [];
        }
    }
}
