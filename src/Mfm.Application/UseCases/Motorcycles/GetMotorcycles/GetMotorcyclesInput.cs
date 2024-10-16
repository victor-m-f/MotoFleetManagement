using Mfm.Application.UseCases.Base;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycles;

public sealed class GetMotorcyclesInput : InputBase<GetMotorcyclesOutput>
{
    public string? LicensePlate { get; }

    public GetMotorcyclesInput(string? licensePlate)
    {
        LicensePlate = licensePlate;
    }
}