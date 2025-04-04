using Microsoft.EntityFrameworkCore;
using Sortec.Application.Interfaces;
using Sortec.Domain.Entities;
using Sortec.Domain.Interface;
using Sortec.Infrastructure.Context;
using Sortec.Infrastructure.Helpers;

namespace Sortec.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly SortecDbContext _context;
        private readonly CryptoHelper _cryptoHelper;
        private readonly IUnitOfWork _unitOfWork;

        public UserServices(SortecDbContext context, CryptoHelper cryptoHelper, IUnitOfWork unitOfWork)
        {
            this._context = context;
            this._cryptoHelper = cryptoHelper;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Response<IEnumerable<User>>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users;
        }

        public async Task<Response<User>> ValidateUser(string userName, string password)
        {
            Response<User> response = new();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());

            if (user == null)
            {
                response.Status = false;
                response.Data = null;
                response.Message = "No existe el usuario";
                return response;
            }

            string decryptedPassword = _cryptoHelper.DecryptString(user.Password);

            if (decryptedPassword != password)
            {
                response.Status = false;
                response.Data = null;
                response.Message = "Credenciales invalidas";
                return response;
            }

            user.LastLoginTimestamp = DateTime.Now;
            await _context.SaveChangesAsync();

            response.Status = true;
            response.Data = user;
            response.Message = $"Se ha encontrado el usuario con id: {user.UserId}";
            return response;
        }
    }
}