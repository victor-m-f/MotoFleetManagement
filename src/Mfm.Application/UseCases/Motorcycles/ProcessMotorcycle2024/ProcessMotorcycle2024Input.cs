using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Base;

namespace Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;

public sealed class ProcessMotorcycle2024Input : InputBase<ProcessMotorcycle2024Output>
{
    public MotorcycleDto Motorcycle { get; }

    public ProcessMotorcycle2024Input(MotorcycleDto motorcycle)
    {
        Motorcycle = motorcycle;
    }
}