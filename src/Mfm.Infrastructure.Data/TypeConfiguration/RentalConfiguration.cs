using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mfm.Infrastructure.Data.TypeConfiguration;

internal sealed partial class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        _ = builder.ToTable(nameof(Rental));
        _ = builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.MotorcycleId)
            .IsRequired();

        builder.Property(x => x.DeliveryPersonId)
            .IsRequired();

        builder.Property(x => x.PlanType)
            .IsRequired();

        builder.Property(x => x.TotalCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(x => x.Motorcycle)
            .WithMany(y => y.Rentals)
            .HasForeignKey(x => x.MotorcycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeliveryPerson)
            .WithMany(y => y.Rentals)
            .HasForeignKey(x => x.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(x => x.Period, periodBuilder =>
        {
            periodBuilder.Property(y => y.StartDate)
                .HasColumnName(nameof(RentalPeriod.StartDate))
                .IsRequired();

            periodBuilder.Property(y => y.ExpectedEndDate)
                .HasColumnName(nameof(RentalPeriod.ExpectedEndDate))
                .IsRequired();

            periodBuilder.Property(y => y.EndDate)
                .HasColumnName(nameof(RentalPeriod.EndDate));
        });
    }
}
