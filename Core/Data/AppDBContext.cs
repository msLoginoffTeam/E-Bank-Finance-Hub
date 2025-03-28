﻿using Core.Data.Models;
using Core_Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using User_Api.Data.Models;
using UserApi.Data.Models;

namespace Core.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Operation> Operations { get; set; }

        public DbSet<CurrencyCourse> CurrencyCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>().HasOne(account => account.Client).WithMany().HasForeignKey("ClientId");

            modelBuilder.Entity<Operation>()
                .UseTphMappingStrategy()
                .HasDiscriminator(Operation => Operation.OperationCategory)
                    .HasValue<CashOperation>(OperationCategory.Cash)
                    .HasValue<CreditOperation>(OperationCategory.Credit)
                    .HasValue<TransferOperation>(OperationCategory.Transfer);
            modelBuilder.Entity<Operation>().HasOne(Operation => Operation.TargetAccount).WithMany().HasForeignKey("TargetAccountId");

            modelBuilder.Entity<TransferOperation>().HasOne(TransferOperation => TransferOperation.SenderAccount).WithMany().HasForeignKey("SenderAccountId");

            modelBuilder.Entity<CurrencyCourse>().HasKey(CurrencyCourse => CurrencyCourse.Currency);

            Client User = new Client()
            {
                Id = new Guid("6e9e5d77-d218-49aa-80a9-3a1f0dba62db")
            };
            modelBuilder.Entity<Client>().HasData(User);
        }
    }
}
