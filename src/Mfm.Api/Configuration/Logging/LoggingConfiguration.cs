using Serilog;
using Serilog.Filters;

namespace Mfm.Api.Configuration.Logging;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder webApplicationBuilder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "MotoFleetManagement")
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .WriteTo.Console()
            .WriteTo.Seq(webApplicationBuilder.Configuration["SeqUrl"]!)
            .Filter.ByExcluding(
            Matching.WithProperty<string>("RequestPath", path => path.StartsWith("/swagger")))
            .CreateLogger();

        webApplicationBuilder.Host.UseSerilog();
    }

    public static void UseLoggingConfiguration(this WebApplication app)
    {
        _ = app.UseSerilogRequestLogging();
    }
}
