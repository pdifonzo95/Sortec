using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sortec.Infrastructure.Middleware
{
    public class JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly RequestDelegate _next = next;
        private readonly IConfiguration _configuration = configuration;

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Autorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Secretkey"));

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var userCalims = new List<Claim>
            {
                new Claim("UserName", jwtToken.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value),
                new Claim(ClaimTypes.NameIdentifier, jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value)
            };

            context.Items["User"] = new ClaimsPrincipal(new ClaimsIdentity(userCalims));
        }
    }
}