using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class AppDbContext:DbContext
    {
        private const string connectionString = @"Data Source=localhost;Initial Catalog=DBTEST;Persist Security Info=True;User ID=phuocnt;Password=phuoc123;Trust Server Certificate=True";

        public AppDbContext()
        {
           // Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<User> User { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<Conversation> Conversation { get; set; }
        public DbSet<ConversationDetail> ConversationDetail { get; set; }
    }
}
