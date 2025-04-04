using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sortec.Infrastructure.Security
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _configuration;

        public JwtProvider(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task<string> GenerateJwtAsync(string id, string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim("UserName", username, ClaimValueTypes.String),
                new Claim(ClaimTypes.NameIdentifier, id, ClaimValueTypes.String)
            };

            var toker = await Task.Run(() =>
            {
                var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Secretkey"));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddHours(8),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(toker);
        }
    }
}