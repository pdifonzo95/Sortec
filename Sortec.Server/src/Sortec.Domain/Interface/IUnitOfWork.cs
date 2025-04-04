using Sortec.Domain.Entities;

namespace Sortec.Domain.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        Task<int> CompleteAsync();
    }
}