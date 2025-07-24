namespace LighthouseSocial.Domain.Interfaces;

public interface ICommentAuditor
{
    ValueTask<bool> IsTextCleanAsync(string text);
}
