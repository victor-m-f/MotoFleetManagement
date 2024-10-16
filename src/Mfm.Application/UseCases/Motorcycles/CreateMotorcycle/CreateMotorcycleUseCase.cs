using MassTransit;
using Mfm.Application.UseCases.Base;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Events;
using Mfm.Domain.Repositories;

namespace Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;

internal sealed class CreateMotorcycleUseCase : UseCaseBase, ICreateMotorcycleUseCase
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IPublishEndpoint _messagePublisher;

    public CreateMotorcycleUseCase(
        IMotorcycleRepository motorcycleRepository,
        IPublishEndpoint messagePublisher)
    {
        _motorcycleRepository = motorcycleRepository;
        _messagePublisher = messagePublisher;
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
        await _motorcycleRepository.SaveChangesAsync(cancellationToken);

        try
        {

            await _messagePublisher.Publish(
                new MotorcycleCreatedEvent(
                    motorcycle.Id,
                    motorcycle.Year,
                    motorcycle.Model,
                    motorcycle.LicensePlate.Value),
                cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }

        return new CreateMotorcycleOutput();
    }
}
