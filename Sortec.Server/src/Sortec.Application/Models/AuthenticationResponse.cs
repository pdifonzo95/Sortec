namespace Sortec.Application.Models
{
    public class AuthenticationResponse(string token, int userId, string name, string lastName, string email,
                                        string userName)
    {
        public string Token { get; set; } = token;
        public int UserId { get; set; } = userId;
        public string Name { get; set; } = name;
        public string LastName { get; set; } = lastName;
        public string Email { get; set; } = email;
        public string UserName { get; set; } = userName;
    }
}