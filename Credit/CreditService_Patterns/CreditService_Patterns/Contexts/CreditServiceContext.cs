using CreditService_Patterns.Models.dbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CreditService_Patterns.Contexts;

public class CreditServiceContext : DbContext
{
    public CreditServiceContext(DbContextOptions<CreditServiceContext> options) : base(options) { }


    public DbSet<CreditPlanDbModel> Plan { get; set; }
    public DbSet<ClientCreditDbModel> Credit { get; set; }
    public DbSet<CreditPaymentDbModel> Payment { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClientCreditDbModel>(entity =>
        {
            entity.HasOne(e => e.CreditPlan)
                .WithMany()
                .HasForeignKey(e => e.CreditPlanId)
                .IsRequired();
        });

        modelBuilder.Entity<CreditPaymentDbModel>(entity =>
        {
            entity.HasOne(e => e.ClientCredit)
                .WithMany()
                .HasForeignKey(e => e.ClientCreditId)
                .IsRequired();
        });
    }
}