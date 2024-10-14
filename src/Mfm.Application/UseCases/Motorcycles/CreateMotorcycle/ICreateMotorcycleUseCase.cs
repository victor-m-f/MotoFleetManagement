using MediatR;

namespace Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;

public interface ICreateMotorcycleUseCase : IRequestHandler<CreateMotorcycleInput, CreateMotorcycleOutput>
{
}
