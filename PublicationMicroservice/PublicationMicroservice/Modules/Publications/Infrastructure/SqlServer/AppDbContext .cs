using Microsoft.EntityFrameworkCore;
using PublicationMicroservice.Modules.Publications.Infrastructure.SqlServer.Entities;

namespace PublicationMicroservice.Modules.Publications.Infrastructure.SqlServer
{
    public class AppDbContext : DbContext
    {
        public DbSet<SqlServerPublicationEntity> Publications { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
