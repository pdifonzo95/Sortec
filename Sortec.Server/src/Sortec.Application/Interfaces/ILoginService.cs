using Sortec.Application.Models;
using Sortec.Domain.DTOs.User;
using Sortec.Domain.Entities;

namespace Sortec.Application.Interfaces
{
    public interface ILoginService
    {
        Task<Response<AuthenticationResponse>> LoginAsync(LoginUserDTO loginUserDTO);
        Task<Response> RegisterAsync(RegisterUserDTO registerUserDTO);
    }
}