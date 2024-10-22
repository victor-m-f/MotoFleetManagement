using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using System.Net;

namespace Mfm.Application.UseCases.Rentals.CreateRental;

public sealed class CreateRentalOutput : OutputBase
{
    private const string InvalidCnhTypeErrorMessage = "Delivery person does not have a valid CNH type.";
    private const string MotorcycleUnavailableErrorMessage = "The motorcycle is not available during the requested period.";

    public CreateRentalOutput()
        : base(HttpStatusCode.Created)
    {
    }

    private CreateRentalOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static CreateRentalOutput CreateDeliveryPersonNotFoundError(string deliveryPersonId)
    {
        var output = new CreateRentalOutput(HttpStatusCode.NotFound);
        output.AddError(NotFoundMessage(nameof(DeliveryPerson), deliveryPersonId));
        return output;
    }

    public static CreateRentalOutput CreateInvalidCnhTypeError()
    {
        var output = new CreateRentalOutput(HttpStatusCode.BadRequest);
        output.AddError(InvalidCnhTypeErrorMessage);
        return output;
    }

    public static CreateRentalOutput CreateMotorcycleNotFoundError(string motorcyclyId)
    {
        var output = new CreateRentalOutput(HttpStatusCode.NotFound);
        output.AddError(NotFoundMessage(nameof(Motorcycle), motorcyclyId));
        return output;
    }

    public static CreateRentalOutput CreateMotorcycleUnavailableError()
    {
        var output = new CreateRentalOutput(HttpStatusCode.Conflict);
        output.AddError(MotorcycleUnavailableErrorMessage);
        return output;
    }
}
