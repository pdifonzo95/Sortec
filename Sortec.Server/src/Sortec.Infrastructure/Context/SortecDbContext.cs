using Microsoft.EntityFrameworkCore;

namespace Sortec.Infrastructure.Context
{
    public abstract partial class SortecDbContext : DbContext
    {
        public SortecDbContext(DbContextOptions<SortecDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}