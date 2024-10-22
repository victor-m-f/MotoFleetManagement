using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.Rentals;
public sealed class CompleteRentalDto
{
    [JsonPropertyName("data_devolucao")]
    [Required]
    public required string ReturnDate { get; init; }
}