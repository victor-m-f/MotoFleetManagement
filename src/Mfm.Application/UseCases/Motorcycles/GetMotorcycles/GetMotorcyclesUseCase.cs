using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycles;

internal sealed class GetMotorcyclesUseCase : UseCaseBase, IGetMotorcyclesUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;

    public GetMotorcyclesUseCase(
        ILogger<GetMotorcyclesUseCase> logger,
        IMotorcycleRepository motorcycleRepository)
        : base(logger)
    {
        _motorcycleRepository = motorcycleRepository;
    }

    public async Task<GetMotorcyclesOutput> Handle(GetMotorcyclesInput request, CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

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
