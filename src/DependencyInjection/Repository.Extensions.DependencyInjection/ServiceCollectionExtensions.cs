using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Repository.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureRepository(this IServiceCollection services)
        {
            services.AddTransient<IMediator, Mediator>();

            services.Scan(scan => scan.
            FromApplicationDependencies().
            AddClasses(x => x.AssignableTo(typeof(INotificationHandler<>))).
            UsingRegistrationStrategy(RegistrationStrategy.Skip).
            AsImplementedInterfaces()
            .WithTransientLifetime());

            return services;
        }
    }
}
