using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mfm.Infrastructure.Data.TypeConfiguration;
internal sealed class DeliveryPersonConfiguration : IEntityTypeConfiguration<DeliveryPerson>
{
    public void Configure(EntityTypeBuilder<DeliveryPerson> builder)
    {
        _ = builder.ToTable(nameof(DeliveryPerson));

        _ = builder.HasKey(x => x.Id);

        _ = builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(DeliveryPersonRules.NameMaxLength);

        _ = builder.Property(x => x.Cnpj)
            .IsRequired()
            .HasMaxLength(DeliveryPersonRules.CnpjMaxLength);
        _ = builder.HasIndex(x => x.Cnpj)
            .IsUnique();

        _ = builder.Property(x => x.DateOfBirth)
            .IsRequired();

        _ = builder.OwnsOne(x => x.Cnh, cnh =>
        {
            cnh.Property(y => y.Number)
                .HasColumnName("CnhNumber")
                .IsRequired()
                .HasMaxLength(DeliveryPersonRules.CnhNumberLength);
            cnh.Property(y => y.Type)
                .HasColumnName("CnhType")
                .IsRequired()
                .HasConversion(
                z => z.ToString(),
                z => (CnhType)Enum.Parse(typeof(CnhType), z));

            cnh.HasIndex(y => y.Number).IsUnique();
        });

        _ = builder.Property(x => x.CnhImageUrl)
            .IsRequired(false)
            .HasMaxLength(DeliveryPersonRules.CnhImageUrlLength);
    }
}

