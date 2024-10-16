using Mfm.Application.UseCases.Base;

namespace Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;

public sealed class UpdateMotorcycleLicensePlateInput : InputBase<UpdateMotorcycleLicensePlateOutput>
{
    public string Id { get; }
    public string LicensePlate { get; }

    public UpdateMotorcycleLicensePlateInput(string id, string licensePlate)
    {
        Id = id;
        LicensePlate = licensePlate;
    }
}