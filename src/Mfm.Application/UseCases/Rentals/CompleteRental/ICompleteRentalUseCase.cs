using MediatR;

namespace Mfm.Application.UseCases.Rentals.CompleteRental;

public interface ICompleteRentalUseCase : IRequestHandler<CompleteRentalInput, CompleteRentalOutput>
{
}
