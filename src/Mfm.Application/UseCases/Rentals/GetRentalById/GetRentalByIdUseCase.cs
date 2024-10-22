using Mfm.Application.Dtos.Rentals;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Repositories;
using Mfm.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Rentals.GetRentalById;

internal sealed class GetRentalByIdUseCase : UseCaseBase, IGetRentalByIdUseCase
{
    private readonly IRentalRepository _rentalRepository;

    public GetRentalByIdUseCase(
        ILogger<GetRentalByIdUseCase> logger,
        IRentalRepository rentalRepository)
        : base(logger)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<GetRentalByIdOutput> Handle(
        GetRentalByIdInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var rental = await _rentalRepository.GetByIdAsync(
            request.Id,
            cancellationToken: cancellationToken);

        if (rental is null)
        {
            return GetRentalByIdOutput.CreateNotFoundError(request.Id);
        }

        var plan = RentalPlan.GetPlan(rental.PlanType);

        var rentalDto = new RentalDto
        {
            Id = rental.Id,
            DailyRate = plan.DailyRate,
            DeliveryPersonId = rental.DeliveryPersonId,
            MotorcycleId = rental.MotorcycleId,
            StartDate = rental.Period.StartDate,
            EndDate = rental.Period.EndDate,
            ExpectedEndDate = rental.Period.ExpectedEndDate,
            ReturnDate = rental.ReturnDate,
        };

        return new GetRentalByIdOutput(rentalDto);
    }
}