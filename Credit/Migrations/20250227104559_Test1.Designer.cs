﻿// <auto-generated />
using System;
using CreditService_Patterns.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CreditService_Patterns.Migrations
{
    [DbContext(typeof(CreditServiceContext))]
    [Migration("20250227104559_Test1")]
    partial class Test1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CreditService_Patterns.Models.dbModels.ClientCreditDbModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ClosingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreditPlanId")
                        .HasColumnType("uuid");

                    b.Property<float>("RemainingAmount")
                        .HasColumnType("real");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CreditPlanId");

                    b.ToTable("Credit");
                });

            modelBuilder.Entity("CreditService_Patterns.Models.dbModels.CreditPaymentDbModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ClientCreditId")
                        .HasColumnType("uuid");

                    b.Property<float>("PaymentAmount")
                        .HasColumnType("real");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ClientCreditId");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("CreditService_Patterns.Models.dbModels.CreditPlanDbModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("PlanName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("PlanPercent")
                        .HasColumnType("real");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Plan");
                });

            modelBuilder.Entity("CreditService_Patterns.Models.dbModels.ClientCreditDbModel", b =>
                {
                    b.HasOne("CreditService_Patterns.Models.dbModels.CreditPlanDbModel", "CreditPlan")
                        .WithMany()
                        .HasForeignKey("CreditPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreditPlan");
                });

            modelBuilder.Entity("CreditService_Patterns.Models.dbModels.CreditPaymentDbModel", b =>
                {
                    b.HasOne("CreditService_Patterns.Models.dbModels.ClientCreditDbModel", "ClientCredit")
                        .WithMany()
                        .HasForeignKey("ClientCreditId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClientCredit");
                });
#pragma warning restore 612, 618
        }
    }
}
