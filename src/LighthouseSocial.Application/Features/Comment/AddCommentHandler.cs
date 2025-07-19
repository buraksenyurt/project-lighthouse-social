using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Comment;

public class AddCommentHandler(ICommentRepository repository,IValidator<CommentDto> validator)
{
    private readonly ICommentRepository _repository = repository;
    private readonly IValidator<CommentDto> _validator = validator;

    public async Task<Result<Guid>> HandleAsync(CommentDto dto)
    {
        //todo@buraksenyurt Aşağıdaki kullanım şeklide diğer handle metotlarında da aynı. Kod tekrarını nasıl önleriz?
        var validation= _validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Result<Guid>.Fail(errors);
        }

        var comment = new Domain.Entities.Comment(dto.UserId, dto.PhotoId, dto.Text, Rating.FromValue(dto.Rating));

        await _repository.AddAsync(comment);

        return Result<Guid>.Ok(comment.Id);
    }
}
