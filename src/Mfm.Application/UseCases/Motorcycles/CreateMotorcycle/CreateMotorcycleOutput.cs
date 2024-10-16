using Mfm.Application.UseCases.Base;
using System.Net;

namespace Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;

public sealed class CreateMotorcycleOutput : OutputBase
{
    public const string SameLicensePlateErrorMessage = "The provided license plate already exists. Please use a unique license plate.";

    public CreateMotorcycleOutput()
        : base(HttpStatusCode.Created)
    {
    }

    private CreateMotorcycleOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static CreateMotorcycleOutput CreateSameLicensePlateError()
    {
        var output = new CreateMotorcycleOutput(HttpStatusCode.BadRequest);
        output.AddError(SameLicensePlateErrorMessage);
        return output;
    }
}
