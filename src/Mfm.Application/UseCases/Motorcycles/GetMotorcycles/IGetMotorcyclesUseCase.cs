using MediatR;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycles;

public interface IGetMotorcyclesUseCase : IRequestHandler<GetMotorcyclesInput, GetMotorcyclesOutput>
{
}
