using Microsoft.Extensions.Logging;
using Repository.Extensions;
using Repository.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.UOW
{
    /// <summary>
    /// UnitOfWork抽象类
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork
    {
        // 保存从数据库得到的原始对象
        private List<BaseEntity> _originalEntities;
        protected IReadOnlyList<BaseEntity> OriginalEntities => _originalEntities?.AsReadOnly();

        // 保存改变了的对象及其改变方式
        private List<EntityTracker> _entityTrackers;
        public IReadOnlyList<EntityTracker> EntityTrackers => _entityTrackers?.AsReadOnly();

        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork()
        {
            Init();
        }

        public UnitOfWork(ILogger<UnitOfWork> logger) 
            : this()
        {
            _logger = logger;
        }

        private void Init()
        {
            _originalEntities = new List<BaseEntity>();
            _entityTrackers = new List<EntityTracker>();
        }

        public BaseEntity Add(BaseEntity entity)
        {
            // 若此待更新对象已被标记为待删除，则不处理，直接返回
            if (HasMarkedDelete(entity))
                return entity;

            // 不能创建已经存在的对象
            if (_originalEntities.FirstOrDefault(oe => oe == entity) != null)
            {
                var msg = "Can't create existed entity. ";
                _logger?.LogError(msg + "{@BaseEntity}", entity);
                throw new ApplicationException($"{msg}{entity}");
            }

            // 记录待创建信息
            if (!_entityTrackers.Any(et => et.Entity == entity))
                _entityTrackers.Add(new EntityTracker { Entity = entity, changeType = ChangeType.Create });

            return entity;
        }

        public IEnumerable<BaseEntity> AddRange(IEnumerable<BaseEntity> entities)
        {
            if (entities == null)
                return entities;

            entities.ToList().ForEach(e => Add(e));

            return entities;
        }

        public void Update(BaseEntity entity)
        {
            // 若此待更新对象已被标记为待删除，则不处理，直接返回
            if (HasMarkedDelete(entity))
                return;

            // 只能更新被已存在的对象
            if (_originalEntities.FirstOrDefault(oe => oe == entity) == null)
            {
                var msg = "Can't update untracked entity. ";
                _logger?.LogError(msg + "{@BaseEntity}", entity);
                throw new ApplicationException($"{msg}{entity}");
            }

            // 记录待更新信息
            _entityTrackers = _entityTrackers.Where(et => et.Entity != entity).ToList();
            _entityTrackers.Add(new EntityTracker { Entity = entity, changeType = ChangeType.Update });
        }

        public void Remove(BaseEntity entity, bool cascade = false)
        {
            // 先去掉该对象的所有跟踪信息（若存在）
            _entityTrackers = _entityTrackers.Where(et => et.Entity != entity).ToList();

            // 若是级联删除，去掉所有导航属性对象的跟踪信息
            if (cascade)
            {
                var naviPropertyInfos = entity.GetType().GetPropertyInfos(true);

                naviPropertyInfos.ForEach(pi =>
                {
                    var naviEntities = (IEnumerable<BaseEntity>)pi.GetValue(entity);

                    foreach (var naviEntity in naviEntities)
                        _entityTrackers = _entityTrackers.Where(et => et.Entity != naviEntity).ToList();
                });
            }

            // 记录待删除信息
            _entityTrackers.Add(new EntityTracker { Entity = entity, changeType = cascade ? ChangeType.DeleteCascade : ChangeType.Delete });
        }

        //private void ValidateEntity(BaseEntity entity)
        //{
        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));

        //    var type = entity.GetType();
        //    var idPropertyInfos = type.GetIdPropertyInfos();

        //    foreach (var pi in idPropertyInfos)
        //    {
        //        var value = pi.GetValue(entity);

        //        if (pi.GetValue(entity) == null)
        //            throw new ApplicationException($"Unable to track an entity of type '{type.Name}' because its primary key is null.");

        //        //// 对于Guid类型的主键，如果未设置值则为其设置一个值
        //        //if (pi.PropertyType == typeof(Guid) && value.ToString() == Guid.Empty.ToString())
        //        //    pi.SetValue(entity, Guid.NewGuid());
        //    }
        //}

        private bool HasMarkedDelete(BaseEntity entity)
        {
            return _entityTrackers.Any(et => (et.changeType == ChangeType.Delete || et.changeType == ChangeType.DeleteCascade) && et.Entity == entity);
        }

        /// <summary>
        /// cache the copy of the entity if it is not cached before, and returns the entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void CacheEntity(BaseEntity entity)
        {
            if (!_originalEntities.Contains(entity))
                _originalEntities.Add(entity.Copy());

            if (!_entityTrackers.Any(et => et.Entity == entity))
                _entityTrackers.Add(new EntityTracker { Entity = entity, changeType = ChangeType.NoChange });
        }

        public BaseEntity GetTrackedEntity(BaseEntity entity)
        {
            return _entityTrackers.FirstOrDefault(et => et.changeType != ChangeType.Create && et.Entity == entity)?.Entity ?? entity;
        }

        protected void CleanTrackingInfo()
        {
            Init();
        }

        public abstract Task<bool> SaveAsync(CancellationToken cancellationToken = default);
    }
}
