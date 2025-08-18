using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;

namespace LighthouseSocial.Application.Features.Photo;

internal record GetRawPhotoRequest(string FileName);
internal class GetRawPhotoHandler(IPhotoStorageService storageService)
    : IHandler<GetRawPhotoRequest, Result<Stream>>
{
    public async Task<Result<Stream>> HandleAsync(GetRawPhotoRequest request, CancellationToken cancellationToken)
    {
        var stream = await storageService.GetAsync(request.FileName);
        if (stream is null || stream.Length == 0)
        {
            return Result<Stream>.Fail("Photo not found in storage.");
        }
        return Result<Stream>.Ok(stream);
    }
}
