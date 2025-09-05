using System.Net.Http.Json;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Infrastructure.Auditors;

public class ExternalCommentAuditor(HttpClient httpClient, ILogger<ExternalCommentAuditor> logger)
    : ICommentAuditor
{
    public async Task<Result<bool>> IsTextCleanAsync(string text)
    {
        try
        {
            //todo@buraksenyurt Adres bilgisi runtime sahibi uygulamadan gelmeli
            //todo@buraksenyurt HashiCorp Consul ile Service Discovery entegrasyonu yapılmalı 
            var response = await
                httpClient.PostAsJsonAsync("http://localhost:5005/moderate", new { comment = text });

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("External comment audit service returned error status: {StatusCode}", response.StatusCode);
                return Result<bool>.Fail($"External audit service failed with status: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<AuditResult>();
            bool isClean = result?.IsClean ?? true;

            logger.LogInformation("Comment audit completed. Text length: {TextLength}, IsClean: {IsClean}", text.Length, isClean);
            return Result<bool>.Ok(isClean);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while auditing comment text. Text length: {TextLength}", text?.Length ?? 0);
            return Result<bool>.Fail($"Failed to audit comment: {ex.Message}");
        }
    }
}

public class AuditResult
{
    public bool IsClean { get; set; }
}
