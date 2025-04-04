using Sortec.Domain.Entities;
using Sortec.Domain.Interface;
using Sortec.Infrastructure.Context;

namespace Sortec.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SortecDbContext _context;
        public IGenericRepository<User> Users { get; private set; }

        public UnitOfWork(SortecDbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}