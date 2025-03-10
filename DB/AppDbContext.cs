using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class AppDbContext:DbContext
    {
        private const string connectionString = @"Data Source=DESKTOP-40EEH15\MSSQLSERVER2019;Initial Catalog=DBTEST;Persist Security Info=True;User ID=sa;Password=phuoc123;Trust Server Certificate=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
