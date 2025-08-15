using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ProjectLighthouseSocial.WebApi.Controllers;

public record CreateLighthouseRequest(string Name, int CountryId, double Latitude, double Longitude);

[ApiController]
[Route("api/[controller]")]
public class LighthouseController(ILogger<LighthouseController> logger, ILighthouseService lighthouseService)
    : ControllerBase
{
    [HttpGet("{lighthouseId:guid}", Name = "GetLigthouseById")]
    public async Task<ActionResult<LighthouseDto>> GetByIdAsync(Guid lighthouseId)
    {
        try
        {
            var result = await lighthouseService.GetByIdAsync(lighthouseId);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving lighthouse with ID {LighthouseId}", lighthouseId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateLighthouseRequest request)
    {
        try
        {
            var dto = new LighthouseDto
            (
                Guid.NewGuid(),
                request.Name,
                request.CountryId,
                request.Latitude,
                request.Longitude
            );
            var result = await lighthouseService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return CreatedAtRoute("GetLigthouseById", new { lighthouseId = result.Data }, result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating lighthouse");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("top/{count:int}")]
    public async Task<ActionResult<IEnumerable<LighthouseTopDto>>> GetTopLighthouses(int count)
    {
        try
        {
            var result = await lighthouseService.GetTopAsync(new TopDto(count));
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving top lighthouses");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{lighthouseId:guid}/photos")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosByIdAsync(Guid lighthouseId)
    {
        try
        {
            var result = await lighthouseService.GetPhotosByIdAsync(lighthouseId);
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving photos for lighthouse with ID {LighthouseId}", lighthouseId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{lighthouseId:guid}")]
    public async Task<ActionResult> Update(Guid lighthouseId, [FromBody] CreateLighthouseRequest request)
    {
        try
        {
            var dto = new LighthouseDto
            (
                lighthouseId,
                request.Name,
                request.CountryId,
                request.Latitude,
                request.Longitude
            );
            var result = await lighthouseService.UpdateAsync(lighthouseId, dto);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating lighthouse with ID {LighthouseId}", lighthouseId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{lighthouseId:guid}")]
    public async Task<ActionResult> Delete(Guid lighthouseId)
    {
        try
        {
            var result = await lighthouseService.DeleteAsync(lighthouseId);
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting lighthouse with ID {LighthouseId}", lighthouseId);
            return StatusCode(500, "Internal server error");
        }
    }
}
