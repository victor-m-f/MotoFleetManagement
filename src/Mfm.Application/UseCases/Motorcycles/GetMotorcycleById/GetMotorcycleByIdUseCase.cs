using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Motorcycles.GetMotorcycleById;

internal sealed class GetMotorcycleByIdUseCase : UseCaseBase, IGetMotorcycleByIdUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;

    public GetMotorcycleByIdUseCase(
        ILogger<GetMotorcycleByIdUseCase> logger,
        IMotorcycleRepository motorcycleRepository)
        : base(logger)
    {
        _motorcycleRepository = motorcycleRepository;
    }

    public async Task<GetMotorcycleByIdOutput> Handle(
        GetMotorcycleByIdInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var motorcycle = await _motorcycleRepository.GetByIdAsync(
            request.Id,
            cancellationToken: cancellationToken);

        if (motorcycle == null)
        {
            return GetMotorcycleByIdOutput.CreateNotFoundError(request.Id);
        }

        return new GetMotorcycleByIdOutput(
            new MotorcycleDto
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year,
                LicensePlate = motorcycle.LicensePlate.Value,
                Model = motorcycle.Model,
            });
    }
}
