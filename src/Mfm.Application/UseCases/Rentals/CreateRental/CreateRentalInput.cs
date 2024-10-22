using Mfm.Application.Dtos.Rentals;
using Mfm.Application.UseCases.Base;

namespace Mfm.Application.UseCases.Rentals.CreateRental;

public sealed class CreateRentalInput : InputBase<CreateRentalOutput>
{
    public CreateRentalDto Rental { get; }

    public CreateRentalInput(CreateRentalDto rental)
    {
        Rental = rental;
    }
}