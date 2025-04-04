using Sortec.Application.Interfaces;
using Sortec.Application.Models;
using Sortec.Domain.DTOs.User;
using Sortec.Domain.Entities;
using Sortec.Domain.Interface;
using Sortec.Infrastructure.Context;
using Sortec.Infrastructure.Helpers;
using Sortec.Infrastructure.Security;

namespace Sortec.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly SortecDbContext _context;
        private readonly CryptoHelper _cryptoHelper;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserServices _userServices;

        public LoginService(SortecDbContext context, CryptoHelper cryptoHelper, IJwtProvider jwtProvider,
                            IUnitOfWork unitOfWork, IUserServices userServices) 
        {
            this._context = context;
            this._cryptoHelper = cryptoHelper;
            this._jwtProvider = jwtProvider;
            this._unitOfWork = unitOfWork;
            this._userServices = userServices;
        }

        public async Task<Response<AuthenticationResponse>> LoginAsync(LoginUserDTO loginUserDTO)
        {
            Response<AuthenticationResponse> response = new();

            try
            {
                var validateUser = await _userServices.ValidateUser(loginUserDTO.UserName, loginUserDTO.Password);

                if (validateUser.Data != null && validateUser.Status)
                {
                    var token = await _jwtProvider.GenerateJwtAsync(validateUser.Data.UserId.ToString(), loginUserDTO.UserName);

                    var authenticationReponse = new AuthenticationResponse(
                        token,
                        validateUser.Data.UserId,
                        validateUser.Data.Name,
                        validateUser.Data.LastName,
                        validateUser.Data.Email,
                        validateUser.Data.UserName
                    );

                    response.Status = true;
                    response.Data = authenticationReponse;
                    response.Message = "Usuario logeado correctamente";
                }
            }
            catch (Exception ex) 
            {
                response.Status = true;
                response.Message = "Ha ocurrido un error: " + ex.Message;
            }

            return response;
        }

        public async Task<Response> RegisterAsync(RegisterUserDTO registerUserDTO)
        {
            Response response = new();

            try
            {
                var registerValidator = await _unitOfWork.Users.GetAllAsync();

                if (registerValidator.Data.Any(x => x.UserName.ToLower() == registerUserDTO.UserName.ToLower()))
                {
                    response.Message = "Ya existe un usuario con el mismo nombre de usuario registrado";
                    response.Status = false;
                }
                else if (registerValidator.Data.Any(x => x.Email.ToLower() == registerUserDTO.Email.ToLower()))
                {
                    response.Message = "Ya existe un usuario con el mismo mail registrado";
                    response.Status = false;
                }

                var user = new User
                {
                    Name = registerUserDTO.Name,
                    LastName = registerUserDTO.LastName,
                    Email = registerUserDTO.Email,
                    Password = _cryptoHelper.EncryptString(registerUserDTO.Password),
                    UserName = registerUserDTO.UserName,
                    PhoneNumber = registerUserDTO.PhoneNumber,
                    CreationTimestamp = DateTime.Now,
                    State = true
                };

                await _unitOfWork.Users.AddAsync(user);

                response.Message = "Usuario registrado con exito";
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Message = "Ha ocurrido un error: " +ex.Message;
                response.Status = false;
            }

            return response;
        }
    }
}