using Mfm.Application.Dtos.Common;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.Rules;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.DeliveryPersons;
public sealed record DeliveryPersonDto
{
    [JsonPropertyName("identificador")]
    [Required]
    public required string Id { get; init; }
    [JsonPropertyName("nome")]
    [Required]
    [StringLength(
        DeliveryPersonRules.NameMaxLength)]
    public required string Name { get; init; }
    [Required]
    [StringLength(
        DeliveryPersonRules.CnpjMaxLength + 4, MinimumLength = DeliveryPersonRules.CnpjMaxLength)]
    public required string Cnpj { get; init; }
    [JsonPropertyName("data_nascimento")]
    [Required]
    public required DateTime DateOfBirth { get; init; }
    [JsonPropertyName("numero_cnh")]
    [Required]
    [StringLength(
        DeliveryPersonRules.CnhNumberLength)]
    public required string CnhNumber { get; init; }
    [JsonPropertyName("tipo_cnh")]
    [Required]
    public required CnhType CnhType { get; init; }
    [JsonPropertyName("imagem_cnh")]
    [Required]
    [ValidImageFormat("png", "bmp")]
    public required string CnhImage { get; init; }
}
