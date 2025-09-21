using LighthouseSocial.Backoffice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient("LighthouseServiceClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["LighthouseService:BaseUrl"] ?? "http://localhost:5072/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<ILigthouseServiceClient, LigthouseServiceClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
