namespace Mfm.Application.Dtos.Motorcycles;
public sealed record MotorcycleDto
{
    public required string Id { get; set; }
    public int Year { get; set; }
    public required string Model { get; set; }
    public required string LicensePlate { get; set; }
}
