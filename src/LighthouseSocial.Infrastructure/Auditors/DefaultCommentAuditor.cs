using LighthouseSocial.Application.Contracts;

namespace LighthouseSocial.Infrastructure.Auditors;

public class DefaultCommentAuditor
    : ICommentAuditor
{
    private static readonly string[] BannedWords = ["badword", "racist", "curse"];
    public Task<bool> IsTextCleanAsync(string text)
    {
        return Task.FromResult(!BannedWords.Any(w => text.Contains(w, StringComparison.OrdinalIgnoreCase)));
    }
}
