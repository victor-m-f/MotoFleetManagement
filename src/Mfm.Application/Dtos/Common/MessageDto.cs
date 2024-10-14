using System.Text.Json.Serialization;

namespace Mfm.Application.Dtos.Common;
public sealed record MessageDto
{
    [JsonPropertyName("mensagem")]
    public required string Message { get; init; }
}
