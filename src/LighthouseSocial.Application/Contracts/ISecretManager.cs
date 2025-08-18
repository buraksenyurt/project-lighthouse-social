namespace LighthouseSocial.Application.Contracts;

public interface ISecretManager
{
    Task<string?> GetSecretAsync(string secretPath, string key);
    Task<Dictionary<string, string>> GetSecretsAsync(string secretPath);
}
