using LighthouseSocial.Backoffice.Models;
using LighthouseSocial.Backoffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace LighthouseSocial.Backoffice.Pages.Lighthouse;

public class CreateModel(ILigthouseServiceClient ligthouseServiceClient, ICountryServiceClient countryServiceClient, IPhotoUploadServiceClient photoUploadServiceClient, ILogger<CreateModel> logger)
    : PageModel
{
    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public LighthouseFormModel LighthouseForm { get; set; } = new LighthouseFormModel();

    [BindProperty]
    [Required(ErrorMessage = "Photo is required")]
    public IFormFile? PhotoFile { get; set; }

    public List<SelectListItem> Countries { get; set; } = [];

    public async Task OnGet()
    {
        LighthouseForm = new LighthouseFormModel();
        await LoadCountriesAsync();
    }

    private async Task LoadCountriesAsync()
    {
        try
        {
            var response = await countryServiceClient.GetAllAsync();

            if (response.Success && response.Data != null)
            {
                Countries = response.Data
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
            }
            else
            {
                Countries = [];
                logger.LogWarning("Failed to load countries: {ErrorMessage}", response.ErrorMessage);
                ErrorMessage = response.ErrorMessage ?? "Failed to load countries.";
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading countries");
            Countries = [];
            ErrorMessage = "An unexpected error occurred while loading countries.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Where(ms => ms.Value?.Errors.Count > 0)
                                   .SelectMany(kvp => kvp.Value.Errors)
                                   .Select(e => e.ErrorMessage)
                                   .ToList();

            logger.LogWarning("ModelState validation failed. Errors: {@Errors}", errors);

            var errorDetails = string.Join("; ", errors.Select(err => $"{err}"));
            ErrorMessage = "Please fix the following errors: " + errorDetails;

            return Page();
        }

        if (PhotoFile == null)
        {
            ErrorMessage = "Photo is required.";
            return Page();
        }

        var (isValidPhoto, photoErrorMessage) = Helpers.PhotoValidator.Validate(PhotoFile);
        if (!isValidPhoto)
        {
            ErrorMessage = photoErrorMessage;
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

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage ?? "An error occurred while creating the lighthouse.";
                return Page();
            }

            var lighthouseId = result.Data;

            using var stream = PhotoFile.OpenReadStream();
            var fileName = PhotoFile.FileName;

            //todo@buraksenyurt: replace "unknown" fields with actual data
            var photoRequest = new PhotoUploadRequest(
                fileName,
                "Unknown",
                Guid.NewGuid(),
                lighthouseId,
                "Unknown",
                "Unknown"
            );

            var uploadResult = await photoUploadServiceClient.UploadPhotoAsync(photoRequest, stream, fileName);

            if (!uploadResult.Success)
            {
                logger.LogWarning("Photo upload failed for LighthouseId {LighthouseId}: {ErrorMessage}", lighthouseId, uploadResult.ErrorMessage);
                ErrorMessage = "Lighthouse created, but photo upload failed: " + (uploadResult.ErrorMessage ?? "Unknown error");
                return RedirectToPage("/Lighthouse/List");
            }

            SuccessMessage = "Lighthouse and photo uploaded successfully.";
            return RedirectToPage("/Lighthouse/List");
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
