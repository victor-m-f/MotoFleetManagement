using Mfm.Domain.Entities.Rules;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.Motorcycles;
public sealed record MotorcycleDto
{
    [JsonPropertyName("identificador")]
    [Required]
    public required string Id { get; init; }
    [JsonPropertyName("ano")]
    [Required]
    [Range(minimum: MotorcycleRules.MinYear, int.MaxValue)]
    public int Year { get; init; }
    [JsonPropertyName("modelo")]
    [Required]
    [StringLength(
        MotorcycleRules.ModelMaxLength)]
    public required string Model { get; init; }
    [JsonPropertyName("placa")]
    [Required]
    [StringLength(
        MotorcycleRules.LicensePlateMaxLength,
        MinimumLength = MotorcycleRules.LicensePlateMinLength)]
    public required string LicensePlate { get; init; }
}
