using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace LighthouseSocial.ODataApi.Controllers;

public class LighthousesController
    : ODataController
{
    private readonly ILighthouseODataRepository _repository;
    private readonly ILogger<LighthousesController> _logger;
    public LighthousesController(ILighthouseODataRepository repository, ILogger<LighthousesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [EnableQuery]
    public IQueryable<QueryableLighthouseDto> Get()
    {
        _logger.LogInformation("Fetching lighthouses from the repository.");
        return _repository.GetLighthouses();
    }
}
