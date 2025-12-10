using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global collation so Contains/Like works correctly in MySQL
            modelBuilder.UseCollation("utf8mb4_general_ci");

            // Optional per-column collation (recommended)
            modelBuilder.Entity<Product>()
                .Property(p => p.ProductName)
                .HasColumnType("varchar(255)")
                .UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<Product>()
                .Property(p => p.Category)
                .HasColumnType("varchar(255)")
                .UseCollation("varchar(255)")
                .UseCollation("utf8mb4_general_ci");
        }
    }
}
