using Mfm.Domain.Exceptions;

namespace Mfm.Application.Helpers;
internal static class ImageHelper
{
    public static (string fileName, byte[] fileBytes) ConvertBase64ToBytes(string base64String, string id)
    {
        if (string.IsNullOrWhiteSpace(base64String))
        {
            throw new ValidationException("The image string cannot be empty.");
        }

        var prefixMap = new Dictionary<string, string>
        {
            { "data:image/png;base64,", "png" },
            { "data:image/bmp;base64,", "bmp" }
        };

        var prefix = prefixMap.Keys
            .FirstOrDefault(p => base64String.StartsWith(p, StringComparison.OrdinalIgnoreCase));

        var cleanBase64 = prefix == null ? base64String : base64String[prefix.Length..];

        try
        {
            var fileBytes = Convert.FromBase64String(cleanBase64);
            var extension = prefix == null ? "png" : prefixMap[prefix];
            var fileName = $"{id}.{extension}";
            return (fileName, fileBytes);
        }
        catch (FormatException ex)
        {
            throw new ValidationException("The provided image is not a valid base64 string.", ex);
        }
    }
}
