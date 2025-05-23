﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UserApi.Data;

#nullable disable

namespace User_Api.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20250413093204_DeviceToken")]
    partial class DeviceToken
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("UserApi.Data.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DeviceToken")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                            Email = "manager@example.com",
                            FullName = "Менеджер А",
                            IsBlocked = false
                        },
                        new
                        {
                            Id = new Guid("6e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                            Email = "user@example.com",
                            FullName = "Клиент Б",
                            IsBlocked = false
                        });
                });

            modelBuilder.Entity("User_Api.Data.Models.UserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "Role");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                            Role = 2
                        },
                        new
                        {
                            UserId = new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                            Role = 0
                        });
                });

            modelBuilder.Entity("User_Api.Data.Models.UserRole", b =>
                {
                    b.HasOne("UserApi.Data.Models.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserApi.Data.Models.User", b =>
                {
                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
