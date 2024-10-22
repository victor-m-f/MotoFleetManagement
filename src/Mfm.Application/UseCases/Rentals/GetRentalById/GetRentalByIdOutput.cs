using Mfm.Application.Dtos.Rentals;
using Mfm.Application.UseCases.Base;
using System.Net;

namespace Mfm.Application.UseCases.Rentals.GetRentalById;

public sealed class GetRentalByIdOutput : OutputBase
{
    public RentalDto? Rental { get; }

    public GetRentalByIdOutput(RentalDto? rental)
        : base(HttpStatusCode.OK)
    {
        Rental = rental;
    }

    private GetRentalByIdOutput(HttpStatusCode httpStatusCode)
        : base(httpStatusCode, false)
    {
    }

    public static GetRentalByIdOutput CreateNotFoundError(string rentalId)
    {
        var output = new GetRentalByIdOutput(HttpStatusCode.NotFound);
        output.AddError(NotFoundMessage(nameof(Rental), rentalId));
        return output;
    }
}
