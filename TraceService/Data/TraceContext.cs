using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TraceService.Data.Models;

namespace TraceService.Data
{
	public class TraceContext : DbContext
	{
		public TraceContext(DbContextOptions<TraceContext> options) : base(options) { }
		public DbSet<TraceDbModel> Traces { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
