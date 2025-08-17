using LighthouseSocial.Application;
using LighthouseSocial.Data;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Infrastructure;
using LighthouseSocial.Infrastructure.Auditors;
using LighthouseSocial.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddData();
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("Minio"));
builder.Services.AddHttpClient<ICommentAuditor, ExternalCommentAuditor>();

// builder.Services.AddOpenApi();

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
