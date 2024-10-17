using MediatR;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycleById;

public interface IGetMotorcycleByIdUseCase : IRequestHandler<GetMotorcycleByIdInput, GetMotorcycleByIdOutput>
{
}
