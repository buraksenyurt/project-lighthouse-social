using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Infrastructure.Auditors;

public class DefaultCommentAuditor(ILogger<DefaultCommentAuditor> logger)
    : ICommentAuditor
{
    private static readonly string[] BannedWords = ["badword", "racist", "curse"];
    public Task<Result<bool>> IsTextCleanAsync(string text)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                logger.LogWarning("Empty or null text provided for comment audit");
                return Task.FromResult(Result<bool>.Ok(true));
            }

            bool isClean = !BannedWords.Any(w => text.Contains(w, StringComparison.OrdinalIgnoreCase));
            logger.LogInformation("Comment audit completed. Text length: {TextLength}, IsClean: {IsClean}", text.Length, isClean);

            return Task.FromResult(Result<bool>.Ok(isClean));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while auditing comment text. Text length: {TextLength}", text?.Length ?? 0);
            return Task.FromResult(Result<bool>.Fail($"Failed to audit comment: {ex.Message}"));
        }
    }
}
