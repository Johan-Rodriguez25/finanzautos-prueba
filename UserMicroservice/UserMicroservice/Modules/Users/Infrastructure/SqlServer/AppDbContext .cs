using Microsoft.EntityFrameworkCore;
using UserMicroservice.Modules.Users.Infrastructure.SqlServer.Entities;

namespace UserMicroservice.Modules.Users.Infrastructure.SqlServer
{
    public class AppDbContext : DbContext
    {
        public DbSet<SqlServerUserEntity> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
