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
        var streamResult = await storageService.GetAsync(request.FileName, cancellationToken);
        if (!streamResult.Success)
        {
            return Result<Stream>.Fail(streamResult.ErrorMessage!);
        }

        var stream = streamResult.Data;
        if (stream is null || stream.Length == 0)
        {
            return Result<Stream>.Fail(Messages.Errors.Photo.PhotoNotFoundInStorage);
        }
        return Result<Stream>.Ok(stream);
    }
}
