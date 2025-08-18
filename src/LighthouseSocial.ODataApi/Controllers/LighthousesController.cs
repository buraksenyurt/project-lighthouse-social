using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LighthouseSocial.ODataApi.Controllers;

public class LighthousesController(ILighthouseODataRepository repository, ILogger<LighthousesController> logger)
    : ODataController
{
    [EnableQuery]
    public IQueryable<QueryableLighthouseDto> Get()
    {
        logger.LogInformation("Fetching lighthouses from the repository.");
        return repository.GetLighthouses();
    }
}
