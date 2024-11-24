using Ingenious.Hackathon.Docusign.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Ingenious.Hackathon.Docusign.Sql
{
    public class HackathonDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Users> Users { get; set; }

        public HackathonDbContext() { }

        public HackathonDbContext(DbContextOptions<HackathonDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.UserUpn)
                .IsUnique();
        }

    }
}
