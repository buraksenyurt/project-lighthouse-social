using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Comment;
internal record AddCommentRequest(CommentDto Comment);

internal class AddCommentHandler(ICommentRepository repository,
    IValidator<CommentDto> validator,
    IUserRepository userRepository,
    IPhotoRepository photoRepository,
    ICommentAuditor commentAuditor
    ) : IHandler<AddCommentRequest, Result<Guid>>
{
    //todo@buraksenyurt İhtiyaç duyulan bileşenlerin daha yönetilebilir ele alınması lazım.
    private readonly ICommentRepository _repository = repository;
    private readonly IValidator<CommentDto> _validator = validator;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPhotoRepository _photoRepository = photoRepository;
    private readonly ICommentAuditor _commentAuditor = commentAuditor;

    public async Task<Result<Guid>> HandleAsync(AddCommentRequest request, CancellationToken cancellationToken)
    {
        var dto = request.Comment;
        var validation = _validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Result<Guid>.Fail(errors);
        }

        var userResult = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (!userResult.Success)
            return Result<Guid>.Fail(userResult.ErrorMessage ?? "User does not exist");

        var photoResult = await _photoRepository.GetByIdAsync(dto.PhotoId, cancellationToken);
        if (!photoResult.Success)
            return Result<Guid>.Fail(photoResult.ErrorMessage!);

        var existsResult = await _repository.ExistsForUserAsync(dto.UserId, dto.PhotoId, cancellationToken);
        if (!existsResult.Success)
            return Result<Guid>.Fail(existsResult.ErrorMessage!);

        if (existsResult.Data!)
            return Result<Guid>.Fail("User has already commented...");

        var isCommentCleanResult = await _commentAuditor.IsTextCleanAsync(dto.Text, cancellationToken);
        if (!isCommentCleanResult.Success)
        {
            return Result<Guid>.Fail(isCommentCleanResult.ErrorMessage!);
        }

        if (!isCommentCleanResult.Data!)
        {
            return Result<Guid>.Fail("Comment text is not appropriate.");
        }

        var comment = new Domain.Entities.Comment(Guid.NewGuid(), dto.UserId, dto.PhotoId, dto.Text, Rating.FromValue(dto.Rating));

        var result = await _repository.AddAsync(comment, cancellationToken);
        if (!result.Success)
        {
            return Result<Guid>.Fail(result.ErrorMessage!);
        }

        return Result<Guid>.Ok(comment.Id);
    }
}
