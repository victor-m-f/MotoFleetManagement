namespace Mfm.Infrastructure.Data.Repositories;
internal abstract class RepositoryBase
{
    protected ApplicationDbContext Context { get; }

    protected RepositoryBase(ApplicationDbContext context)
    {
        Context = context;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}
