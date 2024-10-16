using Mfm.Api.IntegrationTests.Support;
using Mfm.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Mfm.Api.IntegrationTests.Features;

public abstract class FeatureTestsBase : IClassFixture<ApiFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    protected HttpClient HttpClient { get; }
    protected ApplicationDbContext DbContext { get; }

    protected FeatureTestsBase(ApiFactory apiFactory)
    {
        HttpClient = apiFactory.CreateClient();
        _scope = apiFactory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }
}
