using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Events;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Comment;
internal record AddCommentRequest(CommentDto Comment);

internal class AddCommentHandler(ICommentRepository repository,
    IValidator<CommentDto> validator,
    IUserRepository userRepository,
    IPhotoRepository photoRepository,
    ICommentAuditor commentAuditor,
    IEventPublisher eventPublisher
    ) : IHandler<AddCommentRequest, Result<Guid>>
{
    //todo@buraksenyurt İhtiyaç duyulan bileşenlerin daha yönetilebilir ele alınması lazım.
    private readonly ICommentRepository _repository = repository;
    private readonly IValidator<CommentDto> _validator = validator;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPhotoRepository _photoRepository = photoRepository;
    private readonly ICommentAuditor _commentAuditor = commentAuditor;
    private readonly IEventPublisher _eventPublisher = eventPublisher;

    public async Task<Result<Guid>> HandleAsync(AddCommentRequest request, CancellationToken cancellationToken)
    {
        var dto = request.Comment;
        var commentId = Guid.NewGuid();

        var creationRequestedEvent = new CommentCreationRequested(
            commentId,
            dto.UserId,
            dto.PhotoId,
            dto.Text,
            dto.Rating
        );
        await _eventPublisher.PublishAsync(creationRequestedEvent, cancellationToken);

        var validation = _validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            
            var failureEvent = new CommentCreationFailed(
                commentId,
                dto.UserId,
                dto.PhotoId,
                dto.Text,
                dto.Rating,
                "Validation failed",
                errors
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(errors);
        }

        var userResult = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (!userResult.Success)
        {
            var failureEvent = new CommentCreationFailed(
                commentId,
                dto.UserId,
                dto.PhotoId,
                dto.Text,
                dto.Rating,
                "User does not exist",
                userResult.ErrorMessage
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(userResult.ErrorMessage ?? "User does not exist");
        }

        var photoResult = await _photoRepository.GetByIdAsync(dto.PhotoId, cancellationToken);
        if (!photoResult.Success)
        {
            var failureEvent = new CommentCreationFailed(
                commentId,
                dto.UserId,
                dto.PhotoId,
                dto.Text,
                dto.Rating,
                "Photo does not exist",
                photoResult.ErrorMessage
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(photoResult.ErrorMessage!);
        }

        var existsResult = await _repository.ExistsForUserAsync(dto.UserId, dto.PhotoId, cancellationToken);
        if (!existsResult.Success)
        {
            var failureEvent = new CommentCreationFailed(
                commentId,
                dto.UserId,
                dto.PhotoId,
                dto.Text,
                dto.Rating,
                "Error checking existing comment",
                existsResult.ErrorMessage
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(existsResult.ErrorMessage!);
        }

        if (existsResult.Data!)
        {
            var failureEvent = new CommentCreationFailed(
                commentId,
                dto.UserId,
                dto.PhotoId,
                dto.Text,
                dto.Rating,
                "User has already commented on this photo"
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail("User has already commented...");
        }

        var isCommentCleanResult = await _commentAuditor.IsTextCleanAsync(dto.Text, cancellationToken);
        if (!isCommentCleanResult.Success)
        {
            var failureEvent = new CommentCreationFailed(
                commentId,
                dto.UserId,
                dto.PhotoId,
                dto.Text,
                dto.Rating,
                "Comment audit failed",
                isCommentCleanResult.ErrorMessage
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(isCommentCleanResult.ErrorMessage!);
        }

        if (!isCommentCleanResult.Data!)
        {
            var failureEvent = new CommentCreationFailed(
                commentId,
                dto.UserId,
                dto.PhotoId,
                dto.Text,
                dto.Rating,
                "Comment text is not appropriate"
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail("Comment text is not appropriate.");
        }

        var comment = new Domain.Entities.Comment(commentId, dto.UserId, dto.PhotoId, dto.Text, Rating.FromValue(dto.Rating));

        var result = await _repository.AddAsync(comment, cancellationToken);
        if (!result.Success)
        {
            var failureEvent = new CommentCreationFailed(
                commentId,
                dto.UserId,
                dto.PhotoId,
                dto.Text,
                dto.Rating,
                "Database save failed",
                result.ErrorMessage
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(result.ErrorMessage!);
        }

        var successEvent = new CommentCreated(
            comment.Id,
            comment.UserId,
            comment.PhotoId,
            comment.Text,
            comment.Rating.Value,
            comment.CreatedAt
        );
        await _eventPublisher.PublishAsync(successEvent, cancellationToken);

        return Result<Guid>.Ok(comment.Id);
    }
}
