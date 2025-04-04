using Microsoft.AspNetCore.Mvc;
using Sortec.Application.Interfaces;

namespace Sortec.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userServices.GetAllUsersAsync();

            if(response.Status)
                return Ok(response);
            else 
                return BadRequest(response);
        }
    }
}