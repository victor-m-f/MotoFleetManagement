using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mfm.Infrastructure.Data.TypeConfiguration;

internal sealed partial class Motorcycle2024Configuration : IEntityTypeConfiguration<Motorcycle2024>
{
    public void Configure(EntityTypeBuilder<Motorcycle2024> builder)
    {
        _ = builder.ToTable(nameof(Motorcycle2024));

        _ = builder.HasKey(x => x.Id);

        _ = builder.Property(x => x.Year)
            .IsRequired();

        _ = builder.OwnsOne(x => x.LicensePlate, y =>
        {
            y.Property(z => z.Value)
              .HasColumnName(nameof(Motorcycle.LicensePlate))
              .IsRequired()
              .HasMaxLength(MotorcycleRules.LicensePlateMaxLength);
            y.HasIndex(z => z.Value)
            .IsUnique();
        });

        _ = builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(MotorcycleRules.ModelMaxLength);
    }
}
