namespace LighthouseSocial.Domain.Interfaces;

public interface ICommentAuditor
{
    Task<bool> IsTextCleanAsync(string text);
}
