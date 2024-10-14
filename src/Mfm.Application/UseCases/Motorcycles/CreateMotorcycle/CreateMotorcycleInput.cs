using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Base;

namespace Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;

public sealed class CreateMotorcycleInput : InputBase<CreateMotorcycleOutput>
{
    public MotorcycleDto Motorcycle { get; }

    public CreateMotorcycleInput(MotorcycleDto motorcycle)
    {
        Motorcycle = motorcycle;
    }
}