namespace Sortec.Domain.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CompleteAsync();
    }
}