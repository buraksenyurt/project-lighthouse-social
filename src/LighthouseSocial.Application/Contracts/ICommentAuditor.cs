namespace LighthouseSocial.Application.Contracts;

public interface ICommentAuditor
{
    Task<bool> IsTextCleanAsync(string text);
}
