using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ProjectLighthouseSocial.WebApi.Controllers;

public record CreateLighthouseRequest(string Name, int CountryId, double Latitude, double Longitude);

[ApiController]
[Route("api/[controller]")]
public class LighthouseController : ControllerBase
{
    private readonly ILogger<LighthouseController> _logger;
    private readonly ILighthouseService _lighthouseService;

    public LighthouseController(ILogger<LighthouseController> logger, ILighthouseService lighthouseService)
    {
        _logger = logger;
        _lighthouseService = lighthouseService;
    }

    [HttpGet("{lighthouseId:guid}")]
    public async Task<ActionResult<LighthouseDto>> GetByIdAsync(Guid lighthouseId)
    {
        try
        {
            var lighthouse = await _lighthouseService.GetByIdAsync(lighthouseId);
            if (lighthouse == null)
            {
                _logger.LogWarning("Lighthouse with ID {LighthouseId} not found", lighthouseId);
                return NotFound($"Lighthouse with ID {lighthouseId} not found");
            }
            return Ok(lighthouse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lighthouse with ID {LighthouseId}", lighthouseId);
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
            var lighthouseId = await _lighthouseService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByIdAsync), new { lighthouseId }, lighthouseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lighthouse");
            return StatusCode(500, "Internal server error");
        }
    }
}
