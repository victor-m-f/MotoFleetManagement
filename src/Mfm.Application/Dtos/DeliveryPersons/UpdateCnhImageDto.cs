using Mfm.Application.Dtos.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.DeliveryPersons;
public sealed record UpdateCnhImageDto
{
    [JsonPropertyName("imagem_cnh")]
    [Required]
    [ValidImageFormat("png", "bmp")]
    public required string CnhImage { get; init; }
}
