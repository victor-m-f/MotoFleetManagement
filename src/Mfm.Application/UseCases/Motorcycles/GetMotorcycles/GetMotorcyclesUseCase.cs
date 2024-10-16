using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Repositories;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycles;

internal sealed class GetMotorcyclesUseCase : UseCaseBase, IGetMotorcyclesUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;

    public GetMotorcyclesUseCase(
        IMotorcycleRepository motorcycleRepository)
    {
        _motorcycleRepository = motorcycleRepository;
    }

    public async Task<GetMotorcyclesOutput> Handle(GetMotorcyclesInput request, CancellationToken cancellationToken)
    {
        var motorcycles = await _motorcycleRepository.GetMotorcyclesAsync(request.LicensePlate, cancellationToken);

        return new GetMotorcyclesOutput(
            motorcycles.Select(
                x => new MotorcycleDto
                {
                    Id = x.Id,
                    Year = x.Year,
                    LicensePlate = x.LicensePlate.Value,
                    Model = x.Model,
                }));
    }
}
