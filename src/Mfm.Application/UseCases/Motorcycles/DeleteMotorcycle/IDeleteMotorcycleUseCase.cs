using MediatR;

namespace Mfm.Application.UseCases.Motorcycles.DeleteMotorcycle;

public interface IDeleteMotorcycleUseCase : IRequestHandler<DeleteMotorcycleInput, DeleteMotorcycleOutput>
{
}
