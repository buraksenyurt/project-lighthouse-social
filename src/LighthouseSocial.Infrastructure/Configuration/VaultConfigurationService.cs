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
                logger.LogWarning("Database connection string not found in Vault at path: {SecretPath}", SecretPath);
                throw new InvalidOperationException("Database connection string not found in Vault");
            }
            return connectionString;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving database connection string from Vault");
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
                logger.LogWarning("MinIO credentials not found in Vault at path: {SecretPath}", SecretPath);
                throw new InvalidOperationException("MinIO credentials not found in Vault");
            }

            logger.LogInformation("Successfully retrieved MinIO credentials from Vault");
            return (AccessKey: accessKey, SecretKey: secretKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving Minio credentials from Vault");
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
