using Repository.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Repository.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取类型的所有公有实例属性的PropertyInfo
        /// </summary>
        /// <param name="type"></param>
        /// <param name="navi">true：获取导航属性，false：获取非导航属性</param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertyInfos(this Type type, bool navi = false)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => !typeof(BaseEntity).IsAssignableFrom(pi.PropertyType));

            if (navi)
                return properties.Where(pi => typeof(IEnumerable<BaseEntity>).IsAssignableFrom(pi.PropertyType)).ToList();
            else
                return properties.Where(pi => !typeof(IEnumerable<object>).IsAssignableFrom(pi.PropertyType)).ToList();
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            return tableAttribute?.Name ?? $"{type.Name}s";
        }

        /// <summary>
        /// 获取主键属性的PropertyInfo
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetIdPropertyInfos(this Type type)
        {
            var pis = type.GetIdPropertyInfosByAttribute();

            if (pis.Count == 0)
                pis = type.GetIdPropertyInfosByConvention();

            if (pis.Count == 0)
                throw new ArgumentException($"Can't find Id property of Type {type.Name}.");

            return pis;
        }

        // 按KeyAttribute获取主键的PropertyInfo
        private static List<PropertyInfo> GetIdPropertyInfosByAttribute(this Type type)
        {
            var propertyInfos = type.GetPropertyInfos();
            return propertyInfos.Where(pi => pi.GetCustomAttributes<KeyAttribute>().Count() > 0).ToList();
        }

        // 按约定获取主键的PropertyInfo
        // 约定规则：名为Id或TypeName + Id的字段
        private static List<PropertyInfo> GetIdPropertyInfosByConvention(this Type type)
        {
            var idPropertyInfo = type.GetPropertyInfo("Id");

            if (idPropertyInfo == null)
                idPropertyInfo = type.GetPropertyInfo($"{type.Name}Id");

            if (idPropertyInfo == null)
                return new List<PropertyInfo>();

            return new List<PropertyInfo> { idPropertyInfo };
        }

        // 获取某个属性的PropertyInfo，忽略属性名大小写
        private static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
