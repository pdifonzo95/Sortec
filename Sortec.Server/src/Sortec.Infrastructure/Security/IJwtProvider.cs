namespace Sortec.Infrastructure.Security
{
    public interface IJwtProvider
    {
        Task<string> GenerateJwtAsync(string id, string username);
    }
}