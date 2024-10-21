using Mfm.Application.UseCases.Base;
using System.Net;

namespace Mfm.Application.UseCases.DeliveryPersons.CreateDeliveryPerson;

public sealed class CreateDeliveryPersonOutput : OutputBase
{
    public const string SameCnpjErrorMessage = "The provided Cnpj already exists. Please use a unique Cnpj.";
    public const string SameCnhNumberErrorMessage = "The provided Cnh number already exists. Please use a unique Cnh number.";

    public CreateDeliveryPersonOutput()
        : base(HttpStatusCode.Created)
    {
    }

    private CreateDeliveryPersonOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static CreateDeliveryPersonOutput CreateSameCnpjError()
    {
        var output = new CreateDeliveryPersonOutput(HttpStatusCode.BadRequest);
        output.AddError(SameCnpjErrorMessage);
        return output;
    }

    public static CreateDeliveryPersonOutput CreateSameCnhNumberError()
    {
        var output = new CreateDeliveryPersonOutput(HttpStatusCode.BadRequest);
        output.AddError(SameCnhNumberErrorMessage);
        return output;
    }
}
