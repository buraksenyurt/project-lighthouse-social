using LighthouseSocial.Backoffice.Models;
using LighthouseSocial.Backoffice.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LighthouseSocial.Backoffice.Pages.Lighthouse;

public class ListModel(ILigthouseServiceClient ligthouseServiceClient, ILogger<ListModel> logger) : PageModel
{
    public IEnumerable<LighthouseDto> Lighthouses { get; set; } = [];
    public string? ErrorMessage { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public async Task OnGetAsync()
    {
        try
        {
            var pageParam = Request.Query["page"].FirstOrDefault();
            if (int.TryParse(pageParam, out int page))
            {
                CurrentPage = page > 0 ? page : 1;
            }
            else
            {
                CurrentPage = 1;
            }

            var result = await ligthouseServiceClient.GetPagedAsync(CurrentPage, PageSize);

            if (result.Success)
            {
                var pagedResult = result.Data;
                Lighthouses = pagedResult?.Items ?? [];
                TotalCount = pagedResult?.TotalCount ?? 0;
                TotalPages = pagedResult?.TotalPages ?? 0;
                CurrentPage = pagedResult?.CurrentPage ?? CurrentPage;
                PageSize = pagedResult?.PageSize ?? PageSize;

                logger.LogInformation("Successfully fetched lighthouses - Items: {ItemCount}, TotalCount: {TotalCount}, CurrentPage: {CurrentPage}, TotalPages: {TotalPages}",
                    Lighthouses.Count(), TotalCount, CurrentPage, TotalPages);
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "An error occurred while fetching lighthouses.";
                Lighthouses = [];
                logger.LogWarning("Failed to fetch lighthouses: {ErrorMessage}", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while fetching lighthouses.");

            ErrorMessage = "An unexpected error occurred. Please try again later.";
            Lighthouses = [];
        }
    }
}
