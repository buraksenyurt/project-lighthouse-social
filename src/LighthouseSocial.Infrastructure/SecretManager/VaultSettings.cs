namespace LighthouseSocial.Infrastructure.SecretManager;

public class VaultSettings
{
    public string Address { get; set; } = "http://localhost:8200";
    public string Token { get; set; } = string.Empty;
    public string MountPoint { get; set; } = "secret";
}
