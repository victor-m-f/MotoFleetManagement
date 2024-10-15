using Favs.Back.Users.Application.EventHandlers.IntegrationConsumers.Identity;
using MassTransit;
using Mfm.Infrastructure.Messaging.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mfm.Infrastructure.Messaging.Configuration;
public static class MessagingConfiguration
{
    public static void ConfigureMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddMassTransit(x =>
        {
            x.AddConsumer<MotorcycleCreatedConsumer, MotorcycleCreatedConsumerDefinition>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq"));
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
