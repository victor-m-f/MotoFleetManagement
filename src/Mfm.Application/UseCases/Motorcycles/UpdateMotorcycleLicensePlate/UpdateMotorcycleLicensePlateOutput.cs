using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using System.Net;

namespace Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;

public sealed class UpdateMotorcycleLicensePlateOutput : OutputBase
{
    public const string SameLicensePlateErrorMessage = "The provided license plate already exists. Please use a unique license plate.";

    public UpdateMotorcycleLicensePlateOutput()
        : base(HttpStatusCode.NoContent)
    {
    }

    private UpdateMotorcycleLicensePlateOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static UpdateMotorcycleLicensePlateOutput CreateSameLicensePlateError()
    {
        var output = new UpdateMotorcycleLicensePlateOutput(HttpStatusCode.BadRequest);
        output.AddError(SameLicensePlateErrorMessage);
        return output;
    }

    public static UpdateMotorcycleLicensePlateOutput CreateNotFoundError(string motorcycleId)
    {
        var output = new UpdateMotorcycleLicensePlateOutput(HttpStatusCode.NotFound);
        output.AddError(NotFoundMessage(nameof(Motorcycle), motorcycleId));
        return output;
    }
}
