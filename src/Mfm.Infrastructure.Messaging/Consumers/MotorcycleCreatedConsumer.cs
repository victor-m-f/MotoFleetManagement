using MassTransit;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Motorcycles.ProcessMotorcycle2024;
using Mfm.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Mfm.Infrastructure.Messaging.Consumers;
internal sealed class MotorcycleCreatedConsumer
    : IConsumer<MotorcycleCreatedEvent>
{
    private readonly ILogger<MotorcycleCreatedConsumer> _logger;
    private readonly MediatR.IMediator _mediator;

    public MotorcycleCreatedConsumer(ILogger<MotorcycleCreatedConsumer> logger, MediatR.IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<MotorcycleCreatedEvent> context)
    {
        var motorcycleCreatedEvent = context.Message;
        _logger.LogInformation("Received event {event}.", motorcycleCreatedEvent);

        if (motorcycleCreatedEvent.Year != 2024)
        {
            return;
        }

        await _mediator.Send(new ProcessMotorcycle2024Input(
            new MotorcycleDto
            {
                Id = motorcycleCreatedEvent.MotorcycleId,
                Year = motorcycleCreatedEvent.Year,
                LicensePlate = motorcycleCreatedEvent.LicensePlate,
                Model = motorcycleCreatedEvent.Model,
            }));
    }
}
