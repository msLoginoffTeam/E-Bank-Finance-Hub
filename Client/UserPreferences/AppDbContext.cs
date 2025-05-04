using Microsoft.EntityFrameworkCore;
using UserPreferences.Models;

namespace UserPreferences;

public class AppDbContext : DbContext
{
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<HiddenAccount> HiddenAccounts { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> opts)
        : base(opts) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSettings>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<HiddenAccount>();

        base.OnModelCreating(modelBuilder);
    }
}