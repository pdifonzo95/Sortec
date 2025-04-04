using Microsoft.AspNetCore.Mvc;
using Sortec.Application.Interfaces;
using Sortec.Domain.DTOs.User;

namespace Sortec.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public IdentityController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
        {
            var result = await _loginService.LoginAsync(loginUserDTO);

            if(result.Status)
                return Ok(result);
            else 
                return Unauthorized(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUserDTO)
        {
            var result = await _loginService.RegisterAsync(registerUserDTO);

            if (result.Status)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}