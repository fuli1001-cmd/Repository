using Repository.Dapper;
using Meal.Domain.AggregatesModel.IntegrationEventAggregate;

namespace Meal.Infrastructure.Dapper.Repositories
{
    public class IntegrationEventRepository : DapperRepository<IntegrationEvent>, IIntegrationEventRepository
    {
        public IntegrationEventRepository(IntegrationContext context) : base(context) { }
    }
}
