using LighthouseSocial.Backoffice.Models;
using LighthouseSocial.Backoffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LighthouseSocial.Backoffice.Pages.Lighthouse;

public class ListModel : PageModel
{
    private readonly ILogger<ListModel> _logger;
    private readonly ILigthouseServiceClient _lighthouseServiceClient;

    public ListModel(ILigthouseServiceClient ligthouseServiceClient, ILogger<ListModel> logger)
    {
        _lighthouseServiceClient = ligthouseServiceClient;
        _logger = logger;
    }

    public IEnumerable<LighthouseDto> Lighthouses { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var result = await _lighthouseServiceClient.GetPagedAsync(1, 10);

            if (result.Success)
            {
                Lighthouses = result.Data ?? [];
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "An error occurred while fetching lighthouses.";
                Lighthouses = [];
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching lighthouses.");

            ErrorMessage = "An unexpected error occurred. Please try again later.";
            Lighthouses = [];
        }
    }
}
