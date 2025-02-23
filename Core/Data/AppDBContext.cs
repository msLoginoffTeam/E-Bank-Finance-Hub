using Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Operation> Operations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>().HasOne(account => account.Client).WithMany().HasForeignKey("ClientId");

            modelBuilder.Entity<Operation>().UseTphMappingStrategy();
            modelBuilder.Entity<Operation>().HasOne(Operation => Operation.TargetAccount).WithMany().HasForeignKey("TargetAccountId");
            modelBuilder.Entity<Operation>().HasDiscriminator(Operation => Operation.OperationCategory)
                .HasValue<CashOperation>(OperationCategory.Cash)
                .HasValue<CreditOperation>(OperationCategory.Credit);
        }
    }
}
