using MediatR;

namespace Mfm.Application.UseCases.Rentals.CreateRental;

public interface ICreateRentalUseCase : IRequestHandler<CreateRentalInput, CreateRentalOutput>
{
}
