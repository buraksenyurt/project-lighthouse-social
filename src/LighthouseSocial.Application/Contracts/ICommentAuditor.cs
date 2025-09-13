using LighthouseSocial.Application.Common;

namespace LighthouseSocial.Application.Contracts;

public interface ICommentAuditor
{
    Task<Result<bool>> IsTextCleanAsync(string text, CancellationToken cancellationToken = default);
}
