using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Comment;

public class AddCommentHandler(ICommentRepository repository,
    IValidator<CommentDto> validator,
    IUserRepository userRepository,
    IPhotoRepository photoRepository,
    ICommentAuditor commentAuditor
    )
{
    /*
        Bu ve diğer Handler'ların kullandığı bileşen sayısı giderek artabilir.
        Daha yönetilebilir şekilde ele alınmaları lazım. Bir üst contract implementasyonunda,
        düşünülebilir.
    */
    //todo@buraksenyurt İhtiyaç duyulan bileşenlerin daha yönetilebilir ele alınması lazım.
    private readonly ICommentRepository _repository = repository;
    private readonly IValidator<CommentDto> _validator = validator;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPhotoRepository _photoRepository = photoRepository;
    private readonly ICommentAuditor _commentAuditor = commentAuditor;

    public async Task<Result<Guid>> HandleAsync(CommentDto dto)
    {
        //todo@buraksenyurt Aşağıdaki kullanım şeklide diğer handle metotlarında da aynı. Kod tekrarını nasıl önleriz?
        var validation = _validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Result<Guid>.Fail(errors);
        }

        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user is null)
            return Result<Guid>.Fail("User does not exist");

        var photo = await _photoRepository.GetByIdAsync(dto.PhotoId);
        if (photo is null)
            return Result<Guid>.Fail("Photo does not exist");

        var alreadyCommented = await _repository.ExistsForUserAsync(dto.UserId, dto.PhotoId);
        if (alreadyCommented)
            return Result<Guid>.Fail("User has already commented...");

        var isCommentClean = await _commentAuditor.IsTextCleanAsync(dto.Text);
        if (!isCommentClean)
        {
            return Result<Guid>.Fail("Comment contains inappropriate language");
        }

        var comment = new Domain.Entities.Comment(Guid.NewGuid(), dto.UserId, dto.PhotoId, dto.Text, Rating.FromValue(dto.Rating));

        await _repository.AddAsync(comment);

        return Result<Guid>.Ok(comment.Id);
    }
}
