using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;

internal sealed class UpdateMotorcycleLicensePlateUseCase : UseCaseBase, IUpdateMotorcycleLicensePlateUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;

    public UpdateMotorcycleLicensePlateUseCase(
        ILogger<UpdateMotorcycleLicensePlateUseCase> logger,
        IMotorcycleRepository motorcycleRepository)
        : base(logger)
    {
        _motorcycleRepository = motorcycleRepository;
    }

    public async Task<UpdateMotorcycleLicensePlateOutput> Handle(
        UpdateMotorcycleLicensePlateInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (motorcycle is null)
        {
            return UpdateMotorcycleLicensePlateOutput.CreateNotFoundError(request.Id);
        }

        if (motorcycle.LicensePlate.Value == request.LicensePlate)
        {
            return new UpdateMotorcycleLicensePlateOutput();
        }

        var existsMotorcycleWithLicensePlate = await _motorcycleRepository.ExistsMotorcycleWithLicensePlateAsync(
            request.LicensePlate,
            cancellationToken);

        if (existsMotorcycleWithLicensePlate)
        {
            return UpdateMotorcycleLicensePlateOutput.CreateSameLicensePlateError();
        }

        motorcycle.UpdateLicensePlate(new LicensePlate(request.LicensePlate));
        await _motorcycleRepository.SaveChangesAsync(cancellationToken);

        return new UpdateMotorcycleLicensePlateOutput();
    }
}
