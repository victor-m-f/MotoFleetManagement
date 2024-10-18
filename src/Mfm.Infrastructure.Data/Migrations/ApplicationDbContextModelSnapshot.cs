﻿// <auto-generated />
using System;
using Mfm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mfm.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Mfm.Domain.Entities.DeliveryPerson", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CnhImageUrl")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("character varying(14)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("Cnpj")
                        .IsUnique();

                    b.ToTable("DeliveryPerson", (string)null);
                });

            modelBuilder.Entity("Mfm.Domain.Entities.Motorcycle", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Motorcycle", (string)null);
                });

            modelBuilder.Entity("Mfm.Domain.Entities.Motorcycle2024", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Motorcycle2024", (string)null);
                });

            modelBuilder.Entity("Mfm.Domain.Entities.DeliveryPerson", b =>
                {
                    b.OwnsOne("Mfm.Domain.Entities.DeliveryPerson.Cnh#Mfm.Domain.Entities.ValueObjects.Cnh", "Cnh", b1 =>
                        {
                            b1.Property<string>("DeliveryPersonId")
                                .HasColumnType("text");

                            b1.Property<string>("Number")
                                .IsRequired()
                                .HasMaxLength(11)
                                .HasColumnType("character varying(11)")
                                .HasColumnName("CnhNumber");

                            b1.Property<string>("Type")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("CnhType");

                            b1.HasKey("DeliveryPersonId");

                            b1.HasIndex("Number")
                                .IsUnique();

                            b1.ToTable("DeliveryPerson", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("DeliveryPersonId");
                        });

                    b.Navigation("Cnh")
                        .IsRequired();
                });

            modelBuilder.Entity("Mfm.Domain.Entities.Motorcycle", b =>
                {
                    b.OwnsOne("Mfm.Domain.Entities.Motorcycle.LicensePlate#Mfm.Domain.Entities.ValueObjects.LicensePlate", "LicensePlate", b1 =>
                        {
                            b1.Property<string>("MotorcycleId")
                                .HasColumnType("text");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(8)
                                .HasColumnType("character varying(8)")
                                .HasColumnName("LicensePlate");

                            b1.HasKey("MotorcycleId");

                            b1.HasIndex("Value")
                                .IsUnique();

                            b1.ToTable("Motorcycle", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("MotorcycleId");
                        });

                    b.Navigation("LicensePlate")
                        .IsRequired();
                });

            modelBuilder.Entity("Mfm.Domain.Entities.Motorcycle2024", b =>
                {
                    b.OwnsOne("Mfm.Domain.Entities.Motorcycle2024.LicensePlate#Mfm.Domain.Entities.ValueObjects.LicensePlate", "LicensePlate", b1 =>
                        {
                            b1.Property<string>("Motorcycle2024Id")
                                .HasColumnType("text");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(8)
                                .HasColumnType("character varying(8)")
                                .HasColumnName("LicensePlate");

                            b1.HasKey("Motorcycle2024Id");

                            b1.HasIndex("Value")
                                .IsUnique();

                            b1.ToTable("Motorcycle2024", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("Motorcycle2024Id");
                        });

                    b.Navigation("LicensePlate")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
