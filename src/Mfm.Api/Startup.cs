using Mfm.Api.Configuration.ResponseStandardization;
using Mfm.Application.Configuration;
using Mfm.Infrastructure.Data.Configuration;
using Mfm.Infrastructure.Messaging.Configuration;

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
        _ = services.AddControllers();

        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen();
        services.AddSingleton(TimeProvider.System);

        services.ConfigureApplication();
        services.ConfigureData(_configuration);
        services.ConfigureMessaging(_configuration.GetConnectionString("RabbitMq"));
    }

    public void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
            app.ApplyMigrations();
        }

        _ = app.UseMiddleware<ErrorMiddleware>();

        _ = app.UseHttpsRedirection();

        _ = app.UseAuthorization();

        _ = app.MapControllers();
    }
}
