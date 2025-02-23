using Microsoft.EntityFrameworkCore;
using UserApi.Data.Models;

namespace UserApi.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().UseTphMappingStrategy();
            modelBuilder.Entity<User>().HasDiscriminator(User => User.Role)
                .HasValue<Client>(Role.Client)
                .HasValue<Employee>(Role.Employee)
                .HasValue<Manager>(Role.Manager);
        }
    }
}
