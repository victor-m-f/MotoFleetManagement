using Mfm.Api.Configuration.Logging;
using Mfm.Api.Configuration.ResponseStandardization;
using Mfm.Application.Configuration;
using Mfm.Infrastructure.Data.Configuration;
using Mfm.Infrastructure.Messaging.Configuration;
using System.Text.Json.Serialization;
using System.Text.Json;
using Mfm.Infrastructure.Storage.Configuration;

namespace Mfm.Api;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddControllers()
            .AddJsonOptions(
            options => options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen();
        services.AddSingleton(TimeProvider.System);

        services.ConfigureApplication();
        services.ConfigureData(_configuration);
        services.ConfigureMessaging(_configuration.GetConnectionString("RabbitMq"));
        services.ConfigureStorage(_configuration.GetConnectionString("AzureStorage"));
    }

    public void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
            app.ApplyMigrations();
        }

        app.UseLoggingConfiguration();
        _ = app.UseMiddleware<ErrorMiddleware>();

        _ = app.UseHttpsRedirection();
        _ = app.UseAuthorization();
        _ = app.MapControllers();
    }
}
