using LighthouseSocial.Application.Common;

namespace LighthouseSocial.Application.Contracts;

public interface ISecretManager
{
    Task<Result<string?>> GetSecretAsync(string secretPath, string key);
    Task<Result<Dictionary<string, string>>> GetSecretsAsync(string secretPath);
}
