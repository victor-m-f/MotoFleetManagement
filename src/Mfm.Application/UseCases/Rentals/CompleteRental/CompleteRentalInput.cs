using MediatR;

namespace Mfm.Application.UseCases.Rentals.CompleteRental;

public sealed class CompleteRentalInput : IRequest<CompleteRentalOutput>
{
    public string RentalId { get; }
    public string ReturnDateString
    { get; }

    public CompleteRentalInput(string rentalId, string returnDateString)
    {
        RentalId = rentalId;
        ReturnDateString = returnDateString;
    }
}