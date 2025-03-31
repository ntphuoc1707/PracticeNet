using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class AppDbContext:DbContext
    {
        private const string connectionString = @"Data Source=180.77.15.54;Initial Catalog=DBTEST;Persist Security Info=True;User ID=sa;Password=phuoc123;Trust Server Certificate=True";

        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<User> User { get; set; }
    }
}
