using Sortec.Domain.Entities;
using System.Linq.Expressions;

namespace Sortec.Domain.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<Response<IEnumerable<T>>> GetAllAsync();
        Task<Response<T>> GetByIdAsync(int id);
        Task<Response<T>> AddAsync(T entity);
        Task<Response<T>> UpdateAsync(T entity);
        Task<Response<T>> DeleteAsync(int id);
        Task<Response<T>> AddRangeAsync(IEnumerable<T> entities);
        Task<Response<T>> RemoveRangeAsync(IEnumerable<T> entities);
        Task<Response<T>> SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}