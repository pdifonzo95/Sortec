using Sortec.Domain.Entities;

namespace Sortec.Application.Interfaces
{
    public interface IUserServices
    {
        Task<Response<User>> ValidateUser(string userName, string password);
        Task<Response<IEnumerable<User>>> GetAllUsersAsync();
    }
}