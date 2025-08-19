using LighthouseSocial.Application;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Data;
using LighthouseSocial.Infrastructure;
using LighthouseSocial.Infrastructure.Storage;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<QueryableLighthouseDto>("Lighthouses");
    return builder.GetEdmModel();
}

builder.Services.AddControllers()
    .AddOData(options =>
        options.Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("odata", GetEdmModel())
  );

builder.Services
    .AddInfrastructure(builder.Configuration)
    .WithSecretVault()
    .Build();
builder.Services.AddData();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
