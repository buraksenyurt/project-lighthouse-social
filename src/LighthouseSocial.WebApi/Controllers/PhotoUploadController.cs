using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LighthouseSocial.WebApi.Controllers;

public record UploadPhotoRequest(string FileName, string CameraType, Guid UserId, Guid LighthouseId, string Resolution, string Lens);

[ApiController]
[Route("api/[controller]")]
public class PhotoUploadController(ILogger<PhotoController> logger, IPhotoUploadService photoUploadService)
    : ControllerBase
{
    [HttpPost("upload")]
    public async Task<ActionResult<Guid>> UploadAsync([FromForm] UploadPhotoRequest request, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File size exceeds the 5 MB limit.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            if(!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                return BadRequest("Invalid file type. Only JPG and PNG are allowed.");

            using var stream = file.OpenReadStream();
            var dto = new PhotoDto
            (
                Guid.NewGuid(),
                request.FileName,
                DateTime.UtcNow,
                request.CameraType,
                request.UserId,
                request.LighthouseId,
                request.Resolution,
                request.Lens
            );
            
            var result = await photoUploadService.UploadAsync(dto, stream);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading photo for LighthouseId {LighthouseId}", request.LighthouseId);
            return StatusCode(500, Messages.Errors.UnexpectedErrorMessage);
        }
    }
}
