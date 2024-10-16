using Mfm.Api.IntegrationTests.Support;
using Mfm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mfm.Api.IntegrationTests.Features;

public abstract class FeatureTestsBase : IClassFixture<ApiFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    protected HttpClient HttpClient { get; }
    protected ApplicationDbContext DbContext { get; }

    protected FeatureTestsBase(ApiFactory apiFactory)
    {
        _scope = apiFactory.Services.CreateScope();

        HttpClient = apiFactory.CreateClient();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public void Dispose()
    {
        CleanUpDatabaseAsync().Wait();
        _scope?.Dispose();
    }

    private async Task CleanUpDatabaseAsync()
    {
        var connection = DbContext.Database.GetDbConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
        DO $$ DECLARE
        r RECORD;
        BEGIN
            FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
                EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' RESTART IDENTITY CASCADE';
            END LOOP;
        END $$;
    ";

        await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();
    }
}
