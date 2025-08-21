using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
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
