using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Models
{
    internal record CreateLighthouseRequest(LighthouseDto Lighthouse);
    internal record GetLighthouseByIdRequest(Guid Id);
    internal record DeleteLighthouseRequest(Guid id);
    internal record GetAllLighthouseRequest();
}

namespace LighthouseSocial.Application.Features.Photo.Models
{
    internal record UploadPhotoRequest(PhotoDto Photo, Stream Content);
}

namespace LighthouseSocial.Application.Features.Common
{
    internal record EmptyRequest();
}
