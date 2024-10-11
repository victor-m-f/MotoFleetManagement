﻿// <auto-generated />
using System;
using Mfm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mfm.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241011185719_CreateMotorcycle")]
    partial class CreateMotorcycle
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Mfm.Domain.Entities.Motorcycle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Motorcycle", (string)null);
                });

            modelBuilder.Entity("Mfm.Domain.Entities.Motorcycle", b =>
                {
                    b.OwnsOne("Mfm.Domain.ValueObjects.LicensePlate", "LicensePlate", b1 =>
                        {
                            b1.Property<Guid>("MotorcycleId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(7)
                                .HasColumnType("character varying(7)")
                                .HasColumnName("LicensePlate");

                            b1.HasKey("MotorcycleId");

                            b1.HasIndex("Value")
                                .IsUnique();

                            b1.ToTable("Motorcycle");

                            b1.WithOwner()
                                .HasForeignKey("MotorcycleId");
                        });

                    b.Navigation("LicensePlate")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}