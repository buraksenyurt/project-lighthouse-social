using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LighthouseSocial.WebApi.Controllers;

public record UploadPhotoRequest(string FileName, string CameraType, Guid UserId, Guid LighthouseId, string Resolution, string Lens);

[ApiController]
[Route("api/[controller]")]
public class PhotoController(ILogger<PhotoController> logger, IPhotoService photoService, IPhotoUploadService photoUploadService)
    : ControllerBase
{
    //todo@buraksenyurt S6990 bulgusunu çözmek için Interface bazında controller'ları ayrıştır.
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
            
            // var result = await photoService.UploadAsync(dto, stream);

            var result = await photoUploadService.UploadAsync(dto, stream);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading photo for LighthouseId {LighthouseId}", request.LighthouseId);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetRawPhotoAsync(string fileName)
    {
        try
        {
            var result = await photoService.GetRawPhotoAsync(fileName);
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            var stream = result.Data!;
            var contentType = fileName.EndsWith(".png") ? "image/png" : "image/jpeg";
            return File(stream, contentType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving photo {FileName}", fileName);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetByUserIdAsync(Guid userId)
    {
        try
        {
            var result = await photoService.GetByUserIdAsync(userId);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving photos for UserId {UserId}", userId);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpGet("details/{photoId:guid}")]
    public async Task<ActionResult<PhotoDto>> GetByIdAsync(Guid photoId)
    {
        try
        {
            var result = await photoService.GetByIdAsync(photoId);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving photo details for PhotoId {PhotoId}", photoId);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpDelete("{photoId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid photoId)
    {
        try
        {
            var result = await photoService.DeleteAsync(photoId);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting photo {PhotoId}", photoId);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }
}
