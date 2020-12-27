using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper;
using System.Text;
using Repository.UOW;
using Microsoft.Extensions.Logging;
using Repository.Extensions;
using MediatR;
using System.Data;
using Repository.ConnectionFactory;
using Repository.SeedWork;

namespace Repository.Dapper
{
    /// <summary>
    /// 用Dapper实现的UnitOfWork
    /// </summary>
    public class DapperUnitOfWork : UnitOfWork
    {
        private IDbConnectionFactory _dbConnectionFactory;
        private readonly IMediator _mediator;
        private readonly ILogger<DapperUnitOfWork> _logger;

        private List<CommandDefinition> _commands;

        public DapperUnitOfWork(IDbConnectionFactory dbConnectionFactory,
            IMediator mediator,
            ILogger<DapperUnitOfWork> logger)
            : base(logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;

            _commands = new List<CommandDefinition>();
        }

        // 以Unit of work的方式向数据库写入所有changes
        public override async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            // 在创建sql命令之前分发domain event，这样可让domain event handler产生的entity修改在同一个事务中提交
            await _mediator.DispatchDomainEventsAsync(this);

            foreach (var et in EntityTrackers)
            {
                if (et.changeType == ChangeType.Create)
                    AddCreateCommand(et.Entity, cancellationToken);
                else if (et.changeType == ChangeType.Update)
                    AddUpdateCommand(et.Entity, cancellationToken);
                else if (et.changeType == ChangeType.Delete)
                    AddDeleteCommand(et.Entity, cancellationToken);
                else if (et.changeType == ChangeType.DeleteCascade)
                    CascadeAddDeleteCommand(et.Entity, et.GetType().GetPropertyInfos(true), cancellationToken);
            }

            if (_commands.Count > 0)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                        TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var connection = await _dbConnectionFactory.CreateDbConnectionAsync())
                    {
                        foreach (var command in _commands)
                            await connection.ExecuteAsync(command);
                    }

                    scope.Complete();
                }
            }

            // 清除执行成功的commands
            _commands.Clear();

            // 清除实体跟踪信息
            CleanTrackingInfo();

            return true;
        }

        // 添加更新命令
        private void AddUpdateCommand(BaseEntity entity, CancellationToken cancellationToken)
        {
            var type = entity.GetType();
            
            // 得到主键
            var idPropertyInfos = type.GetIdPropertyInfos();
            var idNames = idPropertyInfos.Select(pi => pi.Name).ToList();

            // 得到原对象
            var originalEntity = OriginalEntities.FirstOrDefault(oe => oe == entity);
            if (originalEntity == null)
                throw new ApplicationException($"Can't find original entity of {type.Name}");

            var sqlBuilder = new StringBuilder($"update {type.GetTableName()} set ");

            // 只更新改变了的字段
            var propertyInfos = type.GetPropertyInfos();
            propertyInfos.ForEach(pi =>
            {
                if (!idNames.Contains(pi.Name) && pi.GetValue(entity)?.ToString() != pi.GetValue(originalEntity)?.ToString())
                    sqlBuilder.Append($"{pi.Name}=@{pi.Name},");
            });

            if (sqlBuilder[sqlBuilder.Length - 1] == ',')
            {
                // 使用主键创建where条件
                var where = idNames
                    .Aggregate(" where ",
                    (aggregation, next) => aggregation + $"{next}=@{next} and ",
                    aggregation => aggregation.Substring(0, aggregation.Length - 5));

                sqlBuilder.Remove(sqlBuilder.Length - 1, 1).Append(where);

                // 添加命令
                var command = new CommandDefinition(sqlBuilder.ToString(), entity, cancellationToken: cancellationToken);
                _commands.Add(command);
            }    
        }

        // 添加删除命令
        private void AddDeleteCommand(BaseEntity entity, CancellationToken cancellationToken)
        {
            var type = entity.GetType();

            // 使用主键创建where条件
            var idPropertyInfos = type.GetIdPropertyInfos();
            var where = idPropertyInfos
                .Aggregate(" where ",
                (aggregation, next) => aggregation + $"{next.Name}=@{next.Name} and ",
                aggregation => aggregation.Substring(0, aggregation.Length - 5));

            var sql = $"delete from {type.GetTableName()}{where}";

            // 添加命令
            var command = new CommandDefinition(sql, entity, cancellationToken: cancellationToken);
            _commands.Add(command);
        }

        /// <summary>
        /// 级联添加删除命令（递归）
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="naviPropertyInfos">要删除的实体的所有导航属性信息列表</param>
        /// <param name="cancellationToken"></param>
        private void CascadeAddDeleteCommand(BaseEntity entity, List<PropertyInfo> naviPropertyInfos, CancellationToken cancellationToken)
        {
            var type = entity.GetType();

            if (naviPropertyInfos.Count == 0)
            {
                // 没有导航属性，添加删除该entity的命令
                AddDeleteCommand(entity, cancellationToken);
            }
            else
            {
                // 有导航属性，继续级联添加删除命令
                naviPropertyInfos.ForEach(pi =>
                {
                    var naviEntities = ((IEnumerable<BaseEntity>)pi.GetValue(entity)).ToList();

                    if (naviEntities.Count > 0)
                    {
                        var npis = naviEntities[0].GetType().GetPropertyInfos(true);

                        foreach (var naviEntity in naviEntities)
                            CascadeAddDeleteCommand(naviEntity, npis, cancellationToken);
                    }
                });
            }
        }

        // 添加创建命令
        // 对于IEnumerable类型的导航属性也添加创建命令
        private void AddCreateCommand(BaseEntity entity, CancellationToken cancellationToken)
        {
            var type = entity.GetType();

            // add create command for entity
            var propertyInfos = type.GetPropertyInfos();

            var command = GetCreateCommand(propertyInfos, entity, cancellationToken);

            if (command != null)
                _commands.Add(command.Value);

            // add create commands for navigation properties of entity
            var naviPropertyInfos = type.GetPropertyInfos(true);

            naviPropertyInfos.ForEach(pi =>
            {
                var naviEntity = (IEnumerable<BaseEntity>)pi.GetValue(entity);
                naviEntity?.ToList()?.ForEach(e =>
                {
                    if (e != null)
                        AddCreateCommand(e, cancellationToken);
                });
            });
        }

        // 获取创建命令
        private CommandDefinition? GetCreateCommand(List<PropertyInfo> propertyInfos, BaseEntity entity, CancellationToken cancellationToken)
        {
            if (propertyInfos.Count == 0)
                return null;

            var columns = propertyInfos
                .Select(pi => pi.Name)
                .Aggregate("(",
                (aggregation, next) => aggregation + $"{next},",
                aggregation => aggregation.Substring(0, aggregation.Length - 1) + ")");

            var values = propertyInfos
                .Select(pi => pi.Name)
                .Aggregate("(",
                (aggregation, next) => aggregation + $"@{next},",
                aggregation => aggregation.Substring(0, aggregation.Length - 1) + ")");

            var sqlBuilder = new StringBuilder($"insert into {entity.GetType().GetTableName()}");
            sqlBuilder.Append(columns);
            sqlBuilder.Append(" values");
            sqlBuilder.Append(values);

            return new CommandDefinition(sqlBuilder.ToString(), entity, cancellationToken: cancellationToken);
        }

        public async Task<IDbConnection> CreateDbConnectionAsync()
        {
            return await _dbConnectionFactory.CreateDbConnectionAsync();
        }
    }
}
