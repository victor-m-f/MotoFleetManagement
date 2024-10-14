using Mfm.Domain.Entities.Rules;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.Motorcycles;
public sealed record MotorcycleDto
{
    [JsonPropertyName("identificador")]
    [Required]
    public required string Id { get; set; }
    [JsonPropertyName("ano")]
    [Required]
    public int Year { get; set; }
    [JsonPropertyName("modelo")]
    [Required]
    [StringLength(
        MotorcycleRules.ModelMaxLength)]
    public required string Model { get; set; }
    [JsonPropertyName("placa")]
    [Required]
    [StringLength(
        MotorcycleRules.LicensePlateMaxLength,
        MinimumLength = MotorcycleRules.LicensePlateMinLength)]
    public required string LicensePlate { get; set; }
}
