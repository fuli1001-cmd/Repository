using MediatR;
using Newtonsoft.Json;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Repository.SeedWork
{
    /// <summary>
    /// 所有实体的基类
    /// </summary>
    public abstract class BaseEntity
    {
        int? _hash;

        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public BaseEntity Copy()
        {
            return JsonConvert.DeserializeObject(this.ToString(), this.GetType()) as BaseEntity;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool IsTransient()
        {
            //return Id == default;

            var idPropertyInfos = this.GetType().GetIdPropertyInfos();
            return IsTransient(idPropertyInfos);
        }

        private bool IsTransient(IEnumerable<PropertyInfo> idPropertyInfos)
        {
            bool transient = false;

            foreach (var pi in idPropertyInfos)
            {
                transient = (pi.GetValue(this) == default);

                if (transient)
                    break;
            }

            return transient;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BaseEntity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            BaseEntity item = (BaseEntity)obj;
            var idPropertyInfos = this.GetType().GetIdPropertyInfos();

            if (item.IsTransient(idPropertyInfos) || this.IsTransient(idPropertyInfos))
            {
                return false;
            }
            else
            {
                //return item.Id == Id;

                bool equal = false;

                foreach (var pi in idPropertyInfos)
                {
                    equal = pi.GetValue(obj)?.ToString() == pi.GetValue(this)?.ToString();

                    if (!equal)
                        break;
                }

                return equal;
            }
        }

        public override int GetHashCode()
        {
            var idPropertyInfos = this.GetType().GetIdPropertyInfos();

            if (!IsTransient(idPropertyInfos))
            {
                if (!_hash.HasValue)
                {
                    //_hash = Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                    // calculate hashcode, use prime number to minimise collisions 
                    // https://stackoverflow.com/questions/371328/why-is-it-important-to-override-gethashcode-when-equals-method-is-overridden
                    _hash = 23;
                    idPropertyInfos.ForEach(pi => _hash = _hash * 37 + pi.GetValue(this).GetHashCode());
                }

                return _hash.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(BaseEntity left, BaseEntity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(BaseEntity left, BaseEntity right)
        {
            return !(left == right);
        }
    }
}
