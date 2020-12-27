# Repository

A repository library for .NET core, also implemented unit of work and domain event pattern, currently implemented with Dapper and EFCore.

## Installation
```
// Dapper
dotnet add package Fuli.Repository.Dapper.Extensions.DependencyInjection

// Or use EFCore
dotnet add package Fuli.Repository.Extensions.DependencyInjection
dotnet add package Fuli.Repository.EFCore
```

## Usage

1. Add repository service to IoC comtainer.

    ```c#
    public void ConfigureServices(IServiceCollection services)
    {
        // Dapper
        services.ConfigureDapperRepository();

        // Or use EFCore
        services.ConfigureRepository();
    }
    ```

2. Define your db context.

    ```c#
    // Dapper
    public class MealContext : DapperUnitOfWork
    {
        public MealContext(IMediator mediator, ILogger<MealContext> logger, IDbConnectionFactory dbConnectionFactory) 
            : base(dbConnectionFactory, mediator, logger)
        {
        }
    }

    // Or use EFCore
    public class MealContext : EFUnitOfWork
    {
        public MealContext(DbContextOptions<MealContext> options, IMediator mediator) 
            : base(options, mediator)
        {
        }
    }
    ```

3. Define your entity.

    ```c#
    // Must derive from BaseEntity
    // Use aggregate pattern
    public class Order : BaseEntity1<Guid>, IAggregateRoot { }
    ```

4. Define your repository.

    ```c#
    // Interface
    public interface IOrderRepository : IRepository<Order> 
    {
        Task<Order> GetOrderWithLines(Guid id);
    }

    // Dapper
    public class OrderRepository : DapperRepository<Order>, IOrderRepository
    {
        public OrderRepository(MealContext context) : base(context) { }

        // Use Tracking attribute if you want to track returned entity
        [Tracking]
        public async Task<Order> GetOrderWithLines(Guid id) 
        { 
            // use dapper to query / update database    
        }
    }

    // Or use EFCore
    public class OrderRepository : EfRepository<Order, MealContext>, IOrderRepository 
    { 
        public OrderRepository(MealContext context) : base(context) { }

        public async Task<Order> GetOrderWithLines(Guid id) 
        {
            // use ef core to query / update database 
        }
    }
    ```

5. Dependency Injection.

    ```c#
    public void ConfigureServices(IServiceCollection services)
    {
        // Dapper
        services.AddScoped(sp => ActivatorUtilities.CreateInstance<MealContext>(sp, new SqlServerDbConnectionFactory(configuration.GetConnectionString("MealConnection"))));

        // Or use EFCore
        services.AddDbContext<MealContext>(options =>
        {
            // use SqlServer, or use MySql / MariaDb with UseMySql
            options.UseSqlServer(configuration.GetConnectionString("MealConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
        });

        // register repositories
        services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
    }
    ```

    For more information, please check the sample project.
