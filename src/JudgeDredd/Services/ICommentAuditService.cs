namespace JudgeDredd.Services;

public interface ICommentAuditService
{
    Task<bool> IsFlagged(string comment);
}
