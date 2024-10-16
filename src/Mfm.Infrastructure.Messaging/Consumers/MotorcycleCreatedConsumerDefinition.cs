using MassTransit;

namespace Mfm.Infrastructure.Messaging.Consumers;
internal sealed class MotorcycleCreatedConsumerDefinition : ConsumerDefinition<MotorcycleCreatedConsumer>
{
    public MotorcycleCreatedConsumerDefinition()
    {
        EndpointName = "motorcycle-created";
        ConcurrentMessageLimit = 1;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<MotorcycleCreatedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));
        endpointConfigurator.UseInMemoryOutbox(context);
    }
}