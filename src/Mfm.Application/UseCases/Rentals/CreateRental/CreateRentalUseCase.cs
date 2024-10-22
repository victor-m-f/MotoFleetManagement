using Mfm.Application.Helpers;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Exceptions;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Rentals.CreateRental;

internal sealed class CreateRentalUseCase : UseCaseBase, ICreateRentalUseCase
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly TimeProvider _timeProvider;

    public CreateRentalUseCase(
        ILogger<CreateRentalUseCase> logger,
        IDeliveryPersonRepository deliveryPersonRepository,
        IMotorcycleRepository motorcycleRepository,
        IRentalRepository rentalRepository,
        TimeProvider timeProvider)
        : base(logger)
    {
        _deliveryPersonRepository = deliveryPersonRepository;
        _motorcycleRepository = motorcycleRepository;
        _rentalRepository = rentalRepository;
        _timeProvider = timeProvider;
    }

    public async Task<CreateRentalOutput> Handle(
        CreateRentalInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var deliveryPerson = await _deliveryPersonRepository
            .GetByIdAsync(request.Rental.DeliveryPersonId, cancellationToken);
        if (deliveryPerson == null)
        {
            return CreateRentalOutput.CreateDeliveryPersonNotFoundError(request.Rental.DeliveryPersonId);
        }

        if (!deliveryPerson.IsMotorcycleDriver)
        {
            return CreateRentalOutput.CreateInvalidCnhTypeError();
        }

        var motorcycle = await _motorcycleRepository.GetByIdAsync(
            request.Rental.MotorcycleId,
            includeRentals: true,
            cancellationToken);

        if (motorcycle == null)
        {
            return CreateRentalOutput
                .CreateMotorcycleNotFoundError(request.Rental.MotorcycleId);
        }

        var startDate = request.Rental.StartDate.ToDateTime() 
            ?? throw new ValidationException("StartDate is invalid.");
        var endDate = request.Rental.EndDate.ToDateTime() 
            ?? throw new ValidationException("EndDate is invalid.");
        var expectedEndDate = request.Rental.ExpectedEndDate.ToDateTime() 
            ?? throw new ValidationException("ExpectedEndDate is invalid.");

        if (!motorcycle.IsAvailable(startDate, endDate))
        {
            return CreateRentalOutput.CreateMotorcycleUnavailableError();
        }

        var rental = new Rental(
            request.Rental.MotorcycleId,
            request.Rental.DeliveryPersonId,
            (RentalPlanType)request.Rental.Plan,
            startDate,
            endDate,
            expectedEndDate,
            _timeProvider);

        _rentalRepository.Add(rental);
        await _rentalRepository.SaveChangesAsync(cancellationToken);

        return new CreateRentalOutput();
    }
}
