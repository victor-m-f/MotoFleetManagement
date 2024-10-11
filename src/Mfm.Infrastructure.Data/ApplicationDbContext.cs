using Mfm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mfm.Infrastructure.Data;
internal class ApplicationDbContext : DbContext
{
    public virtual DbSet<Motorcycle> Motorcycles { get; set; } = default!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ApplyAssemblyEntityTypeConfigurations(builder);
    }

    private void ApplyAssemblyEntityTypeConfigurations(ModelBuilder builder)
    {
        var configurationType = typeof(IEntityTypeConfiguration<>);
        var assembly = GetType().Assembly;

        var configTypes = assembly.GetTypes().Where(t =>
            t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == configurationType));

        foreach (var type in configTypes)
        {
            dynamic configurationInstance = Activator.CreateInstance(type)!;
            builder.ApplyConfiguration(configurationInstance);
        }
    }
}
