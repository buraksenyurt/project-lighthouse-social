namespace LighthouseSocial.Infrastructure.Identity;

public class KeycloakSettings
{
    public string Audience { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public int ClockSkew { get; set; } = 5;
    public string Realm { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
}
