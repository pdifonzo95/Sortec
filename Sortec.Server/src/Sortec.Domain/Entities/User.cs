using System.ComponentModel.DataAnnotations;

namespace Sortec.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public byte[] Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public int PhoneNumber { get; set; }
        public bool State { get; set; }
        public DateTime? LastLoginTimestamp { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public string UserName { get; set; }
    }
}