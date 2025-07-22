using OpenAI.Moderations;

namespace JudgeDredd.Services;

public class OpenApiCommentAuditService(IConfiguration configuration)
    : ICommentAuditService
{
    private readonly string _apiKey = configuration["OpenApi:ModerationApiKey"] ?? string.Empty;
    public async Task<bool> IsFlagged(string comment)
    {
        if (string.IsNullOrEmpty(comment))
        {
            return false;
        }

        ModerationClient client = new("omni-moderation-latest", _apiKey);
        var response = await client.ClassifyTextAsync(comment);

        //todo@buraksenyurt Daha önceden flagged=true işaretlenmiş yorumları tekrar sorgulamamak için caching kullanılabilir (Redis mesela)

        return !response.Value.Flagged;
    }
}

public class ModerateRequest
{
    public string Comment { get; set; }
}
