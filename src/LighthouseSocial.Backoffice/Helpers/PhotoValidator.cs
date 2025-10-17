using SixLabors.ImageSharp;

namespace LighthouseSocial.Backoffice.Helpers;

public static class PhotoValidator
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
    private const double AspectRatioTolerance = 0.5;

    public static (bool IsValid, string? ErrorMessage) Validate(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, "No file uploaded.");

        if (file.Length > MaxFileSizeInBytes)
            return (false, $"File size exceeds the {MaxFileSizeInBytes / (1024 * 1024)} MB limit.");

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!AllowedExtensions.Contains(extension))
            return (false, "Invalid file type. Only JPG and PNG are allowed.");

        try
        {
            using var image = Image.Load(file.OpenReadStream());
            var aspectRatio = (double)image.Width / image.Height;

            if (Math.Abs(aspectRatio - 1.0) > AspectRatioTolerance)
                return (false, $"Image aspect ratio is not acceptable. Please upload an image with a more standard aspect ratio. Current ratio: {image.Width}x{image.Height}");
        }
        catch (Exception ex)
        {
            return (false, $"The uploaded file is not a valid image. Error: {ex.Message}");
        }

        return (true, null);
    }
}
