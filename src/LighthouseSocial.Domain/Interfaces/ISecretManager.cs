namespace LighthouseSocial.Domain.Interfaces;

public interface ISecretManager
{
    Task<string?> GetSecretAsync(string secretPath, string key);
    Task<Dictionary<string, string>> GetSecretsAsync(string secretPath);
}
