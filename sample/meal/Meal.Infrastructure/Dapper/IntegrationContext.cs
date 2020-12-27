using MediatR;
using Microsoft.Extensions.Logging;
using Repository.ConnectionFactory;
using Repository.Dapper;

namespace Meal.Infrastructure.Dapper
{
    public class IntegrationContext : DapperUnitOfWork
    {
        public IntegrationContext(IMediator mediator, ILogger<IntegrationContext> logger, IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, mediator, logger)
        {
        }
    }
}
