using System.Net.Http.Json;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Infrastructure.Auditors;

public class ExternalCommentAuditor(HttpClient httpClient)
    : ICommentAuditor
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> IsTextCleanAsync(string text)
    {
        var response = await
            _httpClient.PostAsJsonAsync("https://api.audit/analyze/comment"
            , new { content = text });

        var result = await response.Content.ReadFromJsonAsync<AuditResult>();
        return result?.IsClean ?? true;
    }
}

public class AuditResult
{
    public bool IsClean { get; set; }
}
