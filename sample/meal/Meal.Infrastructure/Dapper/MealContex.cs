using MediatR;
using Microsoft.Extensions.Logging;
using Repository.ConnectionFactory;
using Repository.Dapper;

namespace Meal.Infrastructure.Dapper
{
    public class MealContext : DapperUnitOfWork
    {
        public MealContext(IMediator mediator, ILogger<MealContext> logger, IDbConnectionFactory dbConnectionFactory) 
            : base(dbConnectionFactory, mediator, logger)
        {
        }
    }
}
