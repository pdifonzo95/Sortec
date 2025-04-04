using Microsoft.EntityFrameworkCore;
using Sortec.Domain.Entities;

namespace Sortec.Infrastructure.Context
{
    public class SortecDbContext : DbContext
    {
        public SortecDbContext(DbContextOptions<SortecDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}