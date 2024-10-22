using Mfm.Application.UseCases.Base;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Motorcycles.DeleteMotorcycle;

internal sealed class DeleteMotorcycleUseCase : UseCaseBase, IDeleteMotorcycleUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;

    public DeleteMotorcycleUseCase(
        ILogger<DeleteMotorcycleUseCase> logger,
        IMotorcycleRepository motorcycleRepository)
        : base(logger)
    {
        _motorcycleRepository = motorcycleRepository;
    }

    public async Task<DeleteMotorcycleOutput> Handle(
        DeleteMotorcycleInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var motorcycle = await _motorcycleRepository.GetByIdAsync(
            request.Id,
            includeRentals: true,
            cancellationToken: cancellationToken);

        if (motorcycle is null)
        {
            return DeleteMotorcycleOutput.CreateNotFoundError(request.Id);
        }

        if (motorcycle.Rentals.Count != 0)
        {
            return DeleteMotorcycleOutput.CreateHasRentalRecordsError();
        }

        _motorcycleRepository.Remove(motorcycle);
        await _motorcycleRepository.SaveChangesAsync(cancellationToken);

        return new DeleteMotorcycleOutput();
    }
}
