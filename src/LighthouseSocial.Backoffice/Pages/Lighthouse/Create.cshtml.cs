using LighthouseSocial.Backoffice.Models;
using LighthouseSocial.Backoffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LighthouseSocial.Backoffice.Pages.Lighthouse;

public class CreateModel(ILigthouseServiceClient ligthouseServiceClient, ILogger<CreateModel> logger) : PageModel
{
    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public LighthouseFormModel LighthouseForm { get; set; } = new LighthouseFormModel();

    public void OnGet()
    {
        LighthouseForm = new LighthouseFormModel();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var request = new CreateLighthouseRequest(
                LighthouseForm.Name,
                LighthouseForm.CountryId,
                LighthouseForm.Latitude,
                LighthouseForm.Longitude
            );

            var result = await ligthouseServiceClient.CreateAsync(request);

            if (result.Success)
            {
                SuccessMessage = "Lighthouse created successfully.";
                return RedirectToPage("/Lighthouse/List");
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "An error occurred while creating the lighthouse.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating lighthouse");
            ErrorMessage = "An unexpected error occurred. Please try again later.";

            return Page();
        }
    }
}

public class LighthouseFormModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "CountryId is required")]
    public int CountryId { get; set; }
    [Required(ErrorMessage = "Latitude is required")]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }
    [Required(ErrorMessage = "Longitude is required")]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double Longitude { get; set; }
}
