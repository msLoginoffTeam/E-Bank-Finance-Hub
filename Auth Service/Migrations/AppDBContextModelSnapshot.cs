﻿// <auto-generated />
using System;
using Auth_Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Auth_Service.Migrations
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

            modelBuilder.Entity("Auth_Service.Data.Models.UserAuth", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserAuths");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Auth_Service.Data.Models.ClientAuth", b =>
                {
                    b.HasBaseType("Auth_Service.Data.Models.UserAuth");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.ToTable("ClientAuths");

                    b.HasData(
                        new
                        {
                            Id = new Guid("6e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                            Password = "0362795B2EE7235B3B4D28F0698A85366703EACF0BA4085796FFD980D7653337"
                        });
                });

            modelBuilder.Entity("Auth_Service.Data.Models.EmployeeAuth", b =>
                {
                    b.HasBaseType("Auth_Service.Data.Models.UserAuth");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.ToTable("EmployeeAuths");

                    b.HasData(
                        new
                        {
                            Id = new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                            Password = "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9"
                        });
                });

            modelBuilder.Entity("Auth_Service.Data.Models.ClientAuth", b =>
                {
                    b.HasOne("Auth_Service.Data.Models.UserAuth", null)
                        .WithOne()
                        .HasForeignKey("Auth_Service.Data.Models.ClientAuth", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Auth_Service.Data.Models.EmployeeAuth", b =>
                {
                    b.HasOne("Auth_Service.Data.Models.UserAuth", null)
                        .WithOne()
                        .HasForeignKey("Auth_Service.Data.Models.EmployeeAuth", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
