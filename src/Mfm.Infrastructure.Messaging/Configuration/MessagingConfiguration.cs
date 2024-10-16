using MassTransit;
using Mfm.Infrastructure.Messaging.Consumers;
using Microsoft.Extensions.DependencyInjection;

namespace Mfm.Infrastructure.Messaging.Configuration;
public static class MessagingConfiguration
{
    public static void ConfigureMessaging(
        this IServiceCollection services,
        string? rabbitMqConnectionString)
    {

        services.AddMassTransit(x =>
        {
            x.AddConsumer<MotorcycleCreatedConsumer, MotorcycleCreatedConsumerDefinition>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConnectionString);
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
