using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LighthouseSocial.ODataApi.Controllers;

public class LighthousesController(ILighthouseODataRepository repository, ILogger<LighthousesController> logger)
    : ODataController
{
    [EnableQuery]
    public async Task<IQueryable<QueryableLighthouseDto>> Get()
    {
        logger.LogInformation("Fetching lighthouses from the repository.");

        var result = await repository.GetLighthousesAsync();
        if (!result.Success)
        {
            logger.LogError("Failed to fetch lighthouses: {ErrorMessage}", result.ErrorMessage);
            throw new InvalidOperationException(result.ErrorMessage);
        }

        return result.Data!;
    }
}
