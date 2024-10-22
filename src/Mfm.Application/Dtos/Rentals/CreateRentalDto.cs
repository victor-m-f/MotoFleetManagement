using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.Rentals;
public sealed record CreateRentalDto
{
    [JsonPropertyName("entregador_id")]
    [Required]
    public required string DeliveryPersonId { get; init; }

    [JsonPropertyName("moto_id")]
    [Required]
    public required string MotorcycleId { get; init; }

    [JsonPropertyName("data_inicio")]
    [Required]
    public required string StartDate { get; init; }

    [JsonPropertyName("data_termino")]
    [Required]
    public required string EndDate { get; init; }

    [JsonPropertyName("data_previsao_termino")]
    [Required]
    public required string ExpectedEndDate { get; init; }

    [JsonPropertyName("plano")]
    [Required]
    [Range(7, 50, ErrorMessage = "O plano deve ser 7, 15, 30, 45 ou 50 dias.")]
    public required int Plan { get; init; }
}