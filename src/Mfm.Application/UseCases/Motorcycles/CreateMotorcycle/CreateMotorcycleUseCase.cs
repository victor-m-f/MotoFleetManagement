using MassTransit;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Events;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;

internal sealed class CreateMotorcycleUseCase : UseCaseBase, ICreateMotorcycleUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IPublishEndpoint _messagePublisher;

    public CreateMotorcycleUseCase(
        ILogger<CreateMotorcycleUseCase> logger,
        IMotorcycleRepository motorcycleRepository,
        IPublishEndpoint messagePublisher)
        : base(logger)
    {
        _motorcycleRepository = motorcycleRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<CreateMotorcycleOutput> Handle(
        CreateMotorcycleInput request,
        CancellationToken cancellationToken)
    {
        LogUseCaseExecutionStarted(request);

        var existsMotorcycleWithLicensePlate = await _motorcycleRepository.ExistsMotorcycleWithLicensePlateAsync(
            request.Motorcycle.LicensePlate,
            cancellationToken);

        if (existsMotorcycleWithLicensePlate)
        {
            return CreateMotorcycleOutput.CreateSameLicensePlateError();
        }

        var licensePlate = new LicensePlate(request.Motorcycle.LicensePlate);
        var motorcycle = new Motorcycle(
            request.Motorcycle.Id,
            licensePlate,
            request.Motorcycle.Year,
            request.Motorcycle.Model);

        _motorcycleRepository.Add(motorcycle);
        await _motorcycleRepository.SaveChangesAsync(cancellationToken);

        await _messagePublisher.Publish(
            new MotorcycleCreatedEvent(
                motorcycle.Id,
                motorcycle.Year,
                motorcycle.Model,
                motorcycle.LicensePlate.Value),
            cancellationToken);

        return new CreateMotorcycleOutput();
    }
}
