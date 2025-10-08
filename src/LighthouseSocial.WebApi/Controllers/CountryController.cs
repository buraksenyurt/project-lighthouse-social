using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LighthouseSocial.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController(ILogger<CountryController> logger, ICountryService countryService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CountryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await countryService.GetAllAsync(cancellationToken);

            if (!result.Success)
            {
                logger.LogWarning("Failed to get all countries: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while getting all countries");
            return StatusCode(500, Messages.Errors.InternalServerErrorMessage);
        }
    }
}
