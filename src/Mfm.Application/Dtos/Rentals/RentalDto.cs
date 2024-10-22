using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.Rentals;
public sealed record RentalDto
{
    [JsonPropertyName("identificador")]
    public required string Id { get; init; }

    [JsonPropertyName("valor_diaria")]
    public decimal DailyRate { get; init; }

    [JsonPropertyName("entregador_id")]
    public required string DeliveryPersonId { get; init; }

    [JsonPropertyName("moto_id")]
    public required string MotorcycleId { get; init; }

    [JsonPropertyName("data_inicio")]
    public DateTimeOffset StartDate { get; init; }

    [JsonPropertyName("data_termino")]
    public DateTimeOffset EndDate { get; init; }

    [JsonPropertyName("data_previsao_termino")]
    public DateTimeOffset ExpectedEndDate { get; init; }

    [JsonPropertyName("data_devolucao")]
    public DateTimeOffset? ReturnDate { get; init; }
}