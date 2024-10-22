using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using System.Net;

namespace Mfm.Application.UseCases.Rentals.CompleteRental;

public sealed class CompleteRentalOutput : OutputBase
{
    public const string SuccessMessage = "Data de devolução informada com sucesso";

    public CompleteRentalOutput()
        : base(HttpStatusCode.OK)
    {
    }

    private CompleteRentalOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static CompleteRentalOutput CreateRentalNotFoundError(string rentalId)
    {
        var output = new CompleteRentalOutput(HttpStatusCode.NotFound);
        output.AddError(NotFoundMessage(nameof(Rental), rentalId));
        return output;
    }
}
