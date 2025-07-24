using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Infrastructure.Auditors;

public class DefaultCommentAuditor
    : ICommentAuditor
{
    private static readonly string[] BannedWords = ["badword", "racist", "curse"];
    public ValueTask<bool> IsTextCleanAsync(string text)
    {
        return ValueTask.FromResult(!BannedWords.Any(w => text.Contains(w, StringComparison.OrdinalIgnoreCase)));
    }
}
