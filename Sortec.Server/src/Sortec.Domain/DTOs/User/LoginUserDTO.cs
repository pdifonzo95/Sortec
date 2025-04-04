using System.ComponentModel.DataAnnotations;

namespace Sortec.Domain.DTOs.User
{
    public class LoginUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}