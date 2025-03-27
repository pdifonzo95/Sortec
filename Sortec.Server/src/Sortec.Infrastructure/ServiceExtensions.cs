using Castle.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sortec.Infrastructure.ExeptionHandling;

namespace Sortec.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void ConfigurePersistance(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddExceptionHandler<GlobalExceptionHandler>();
        }
    }
}