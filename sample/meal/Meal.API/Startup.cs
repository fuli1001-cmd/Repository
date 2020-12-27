using System;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Repository.ConnectionFactory;
using Repository.Dapper.Extensions.DependencyInjection;
using Repository.Extensions.DependencyInjection;
using Meal.Application.Commands.CreateOrder;
using Meal.Domain.AggregatesModel.OrderAggregate;
using Meal.API.Application.Validations;
using Meal.Domain.AggregatesModel.MeetingAggregate;
using Meal.API.Application.Behaviors;
using Meal.API.Infrastructure.Filters;
using FluentValidation;
using Meal.API.Application.Commands.Meeting.CreateMeeting;
using Meal.API.Application.Queries;

namespace Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDapper(Configuration)
                //.AddEFCore(Configuration)
                .AddMediatR(typeof(CreateOrderCommand).GetTypeInfo().Assembly)
                .AddScoped<IMeetingQueries, MeetingQueries>()
                .AddScoped<IOrderQueries, OrderQueries>()
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddSingleton(typeof(IValidator<CreateMeetingCommand>), typeof(CreateMeetingCommandValidator))
                .AddAutoMapper(typeof(OrderViewModelProfile).Assembly)
                .AddAutoMapper(typeof(MeetingViewModelProfile).Assembly)
                .AddControllers(options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                })
                .AddNewtonsoftJson();
                //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateMeetingCommandValidator>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureDapperRepository();

            // add typed db context
            services.AddScoped(sp => ActivatorUtilities.CreateInstance<Meal.Infrastructure.Dapper.MealContext>(sp, new SqlServerDbConnectionFactory(configuration.GetConnectionString("MealConnection"))));
            //services.AddScoped(sp => ActivatorUtilities.CreateInstance<Meal.Infrastructure.Dapper.IntegrationContext>(sp, new MySqlDbConnectionFactory(configuration.GetConnectionString("IntegrationConnection"))));

            // add repositories
            services.AddScoped(typeof(IOrderRepository), typeof(Meal.Infrastructure.Dapper.Repositories.OrderRepository));
            services.AddScoped(typeof(IMeetingRepository), typeof(Meal.Infrastructure.Dapper.Repositories.MeetingRepository));
            //services.AddScoped(typeof(IIntegrationEventRepository), typeof(Meal.Infrastructure.Dapper.Repositories.IntegrationEventRepository));

            return services;
        }

        public static IServiceCollection AddEFCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureRepository();

            // use SqlServer
            AddSqlServerDbContext<Meal.Infrastructure.EFCore.MealContext>(services, configuration.GetConnectionString("MealConnection"));
            //AddSqlServerDbContext<Meal.Infrastructure.EFCore.IntegrationContext>(services, configuration.GetConnectionString("IntegrationConnection"));

            // use MariaDb
            //AddMariaDbContext<Meal.Infrastructure.EFCore.TestContext>(services, configuration.GetConnectionString("MealConnection"));
            //AddMariaDbContext<Meal.Infrastructure.EFCore.IntegrationContext>(services, configuration.GetConnectionString("IntegrationConnection"));

            // add repositories
            services.AddScoped(typeof(IOrderRepository), typeof(Meal.Infrastructure.EFCore.Repositories.OrderRepository));
            services.AddScoped(typeof(IMeetingRepository), typeof(Meal.Infrastructure.EFCore.Repositories.MeetingRepository));
            //services.AddScoped(typeof(IIntegrationEventRepository), typeof(Meal.Infrastructure.EFCore.Repositories.IntegrationEventRepository));

            return services;
        }

        private static void AddSqlServerDbContext<T>(IServiceCollection services, string connectionString) where T : DbContext
        {
            services.AddDbContext<T>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });
        }

        private static void AddMariaDbContext<T>(IServiceCollection services, string connectionString) where T : DbContext
        {
            // consider use AddDbContextPool
            services.AddDbContext<T>(options =>
            {
                options.UseMySql(connectionString,
                    new MariaDbServerVersion(new Version(10, 5, 8)),
                    mySqlOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        sqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend);
                    });
            });
        }
    }
}
