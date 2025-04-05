using Microsoft.EntityFrameworkCore;
using User_Api.Data.Models;
using UserApi.Data.Models;

namespace UserApi.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>().HasOne(UserRole => UserRole.User).WithMany(User => User.Roles).HasForeignKey("UserId");
            modelBuilder.Entity<UserRole>().HasKey("UserId", "Role");

            User Manager = new User()
            {
                Id = new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                Email = "manager@example.com",
                FullName = "Менеджер А",
                IsBlocked = false
            };
            modelBuilder.Entity<User>().HasData(Manager);
            modelBuilder.Entity<UserRole>().HasData(new 
            {
                UserId = Manager.Id,
                Role = Role.Manager
            });

            User User = new User()
            {
                Id = new Guid("6e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                Email = "user@example.com",
                FullName = "Клиент Б",
                IsBlocked = false
            };
            modelBuilder.Entity<User>().HasData(User);
            modelBuilder.Entity<UserRole>().HasData(new
            {
                UserId = Manager.Id,
                Role = Role.Client
            });
        }
    }
}
