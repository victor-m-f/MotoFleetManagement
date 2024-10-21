using System.ComponentModel.DataAnnotations;

namespace Mfm.Application.Dtos.Common;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class ValidImageFormatAttribute : ValidationAttribute
{
    private readonly string[] _validExtensions;

    public ValidImageFormatAttribute(params string[] validExtensions)
    {
        _validExtensions = validExtensions.Select(e => e.ToLower()).ToArray();
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string base64String || string.IsNullOrWhiteSpace(base64String))
        {
            return ValidationResult.Success;
        }

        var prefix = GetValidPrefix(base64String);
        if (prefix == null)
        {
            return new ValidationResult(
                $"The image must be in one of the following formats: {string.Join(", ", _validExtensions)}.");
        }

        var cleanBase64 = base64String[prefix.Length..];

        try
        {
            _ = Convert.FromBase64String(cleanBase64);
        }
        catch (FormatException)
        {
            return new ValidationResult("The provided image is not a valid base64 string.");
        }

        return ValidationResult.Success;
    }

    private string? GetValidPrefix(string base64String)
    {
        return _validExtensions
            .Select(ext => $"data:image/{ext};base64,")
            .FirstOrDefault(prefix => base64String.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }
}