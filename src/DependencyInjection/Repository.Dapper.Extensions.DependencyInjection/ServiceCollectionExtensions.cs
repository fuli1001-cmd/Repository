using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Repository.Extensions.DependencyInjection;

namespace Repository.Dapper.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDapperRepository(this IServiceCollection services)
        {
            services.ConfigureRepository();
            services.ConfigureDynamicProxy();

            return services;
        }
    }
}
