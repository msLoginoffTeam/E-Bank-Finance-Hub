﻿// <auto-generated />
using System;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Data.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<float>("BalanceInRubles")
                        .HasColumnType("real");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Core.Data.Models.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Core.Data.Models.Operation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<float>("AmountInRubles")
                        .HasColumnType("real");

                    b.Property<int>("OperationCategory")
                        .HasColumnType("integer");

                    b.Property<int>("OperationType")
                        .HasColumnType("integer");

                    b.Property<Guid>("TargetAccountId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("TargetAccountId");

                    b.ToTable("Operations");

                    b.HasDiscriminator<int>("OperationCategory");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Core.Data.Models.CashOperation", b =>
                {
                    b.HasBaseType("Core.Data.Models.Operation");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Core.Data.Models.CreditOperation", b =>
                {
                    b.HasBaseType("Core.Data.Models.Operation");

                    b.Property<Guid>("CreditId")
                        .HasColumnType("uuid");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Core.Data.Models.Account", b =>
                {
                    b.HasOne("Core.Data.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Core.Data.Models.Operation", b =>
                {
                    b.HasOne("Core.Data.Models.Account", "TargetAccount")
                        .WithMany()
                        .HasForeignKey("TargetAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TargetAccount");
                });
#pragma warning restore 612, 618
        }
    }
}
