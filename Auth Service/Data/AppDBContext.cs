using Auth_Service.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Auth_Service.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<UserAuth> UserAuths { get; set; }
        public DbSet<ClientAuth> ClientAuths { get; set; }
        public DbSet<EmployeeAuth> EmployeeAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAuth>().UseTptMappingStrategy();

            modelBuilder.Entity<EmployeeAuth>().HasData(new EmployeeAuth()
            {
                Id = new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                Password = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("admin123"))),
            });

            modelBuilder.Entity<ClientAuth>().HasData(new ClientAuth()
            {
                Id = new Guid("6e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                Password = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("string123"))),
            });
        }
    }
}
