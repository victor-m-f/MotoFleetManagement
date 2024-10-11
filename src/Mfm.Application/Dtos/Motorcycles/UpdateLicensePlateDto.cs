using Mfm.Domain.Entities.Rules;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.Motorcycles;
public sealed record UpdateLicensePlateDto
{
    [JsonPropertyName("placa")]
    [Required]
    [StringLength(
        MotorcycleRules.LicensePlateMaxLength,
        MinimumLength = MotorcycleRules.LicensePlateMinLength)]
    public required string LicensePlate { get; init; }
}
