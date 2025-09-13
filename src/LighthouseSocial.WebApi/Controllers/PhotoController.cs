using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LighthouseSocial.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotoController(ILogger<PhotoController> logger, IPhotoService photoService)
    : ControllerBase
{
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
            return StatusCode(500, Messages.Errors.UnexpectedErrorMessage);
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
            return StatusCode(500, Messages.Errors.UnexpectedErrorMessage);
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
            return StatusCode(500, Messages.Errors.UnexpectedErrorMessage);
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
            return StatusCode(500, Messages.Errors.UnexpectedErrorMessage);
        }
    }
}
