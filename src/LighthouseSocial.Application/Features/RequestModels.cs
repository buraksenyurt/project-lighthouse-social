using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Models
{
    internal record CreateLighthouseRequest(LighthouseDto Lighthouse);
    internal record DeleteLighthouseRequest(Guid Id);
    internal record GetAllLighthouseRequest();
    internal record GetLighthouseByIdRequest(Guid Id);
}

namespace LighthouseSocial.Application.Features.Photo.Models
{
    internal record DeletePhotoRequest(Guid Id);
    internal record UploadPhotoRequest(PhotoDto Photo, Stream Content);
}

namespace LighthouseSocial.Application.Features.Comment.Models
{
    internal record AddCommentRequest(CommentDto Comment);
    internal record DeleteCommentRequest(Guid CommentId);
    internal record GetCommentByPhotoRequest(Guid PhotoId);
}

namespace LighthouseSocial.Application.Features.Common
{
    internal record EmptyRequest();
}
