using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Favs.Back.Medias.Data.TypeConfiguration;

internal sealed partial class MotorcycleConfiguration : IEntityTypeConfiguration<Motorcycle>
{
    public void Configure(EntityTypeBuilder<Motorcycle> builder)
    {
        _ = builder.ToTable(nameof(Motorcycle));

        builder.OwnsOne(x => x.LicensePlate, y =>
        {
            y.Property(z => z.Value)
              .HasColumnName(nameof(Motorcycle.LicensePlate))
              .IsRequired()
              .HasMaxLength(MotorcycleRules.LicensePlateMaxLength);
            y.HasIndex(z => z.Value)
            .IsUnique(); 
        });

        builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(MotorcycleRules.ModelMaxLength);
    }
}
