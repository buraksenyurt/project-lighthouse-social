using JudgeDredd.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<ICommentAuditService, OpenApiCommentAuditService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/moderate", async (ModerateRequest request, ICommentAuditService auditService) =>
{
    var isClean = await auditService.IsFlagged(request.Comment);
    return Results.Ok(new { isClean });
});


app.Run();

