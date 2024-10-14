using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Entities;
using Mfm.Domain.Repositories;

namespace Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;

public sealed class CreateMotorcycleUseCase : UseCaseBase, ICreateMotorcycleUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;

    public CreateMotorcycleUseCase(IMotorcycleRepository motorcycleRepository)
    {
        _motorcycleRepository = motorcycleRepository;
    }

    public async Task<CreateMotorcycleOutput> Handle(
        CreateMotorcycleInput request,
        CancellationToken cancellationToken)
    {
        var licensePlate = new LicensePlate(request.Motorcycle.LicensePlate);
        var motorcycle = new Motorcycle(
            request.Motorcycle.Id,
            licensePlate,
            request.Motorcycle.Year,
            request.Motorcycle.Model);

        _motorcycleRepository.Add(motorcycle);
        await _motorcycleRepository.SaveChangesAsync();

        return new CreateMotorcycleOutput();
    }
}
