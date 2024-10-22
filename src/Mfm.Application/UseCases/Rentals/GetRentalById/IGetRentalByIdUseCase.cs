using MediatR;

namespace Mfm.Application.UseCases.Rentals.GetRentalById;

public interface IGetRentalByIdUseCase : IRequestHandler<GetRentalByIdInput, GetRentalByIdOutput>
{
}
