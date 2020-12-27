using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Repository.SeedWork;
using Repository.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Repository.Attributes
{
    public class TrackingAttribute : AbstractInterceptorAttribute
    {
        private static readonly MethodInfo _taskResultMethod;

        static TrackingAttribute()
        {
            _taskResultMethod = typeof(Task).GetMethods().FirstOrDefault(p => p.Name == "FromResult" && p.ContainsGenericParameters);
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            // execute method
            await next(context);

            // get return value
            object obj = null;
            if (context.IsAsync())
                obj = await context.UnwrapAsyncReturnValue();
            else
                obj = context.ReturnValue;

            // cache & replace
            var type = obj?.GetType();
            if (type != null)
            {
                if (type.IsSubclassOf(typeof(BaseEntity)))
                {
                    var entity = (BaseEntity)obj;

                    dynamic repository = context.Implementation;
                    var uow = repository.UnitOfWork;

                    uow.CacheEntity(entity);

                    // 使用跟踪的值取代从数据库中查询的值
                    var returnValue = uow.GetTrackedEntity(entity);
                    var returnType = context.IsAsync() ? context.ServiceMethod.ReturnType.GetGenericArguments().First() : context.ServiceMethod.ReturnType;
                    context.ReturnValue = context.IsAsync() ? _taskResultMethod.MakeGenericMethod(returnType).Invoke(null, new object[] { returnValue }) : returnValue;
                }
                else if (typeof(IEnumerable<BaseEntity>).IsAssignableFrom(type))
                {
                    dynamic repository = context.Implementation;
                    var uow = repository.UnitOfWork;

                    // 取得entity类型
                    var entityType = context.IsAsync() ? 
                        context.ServiceMethod.ReturnType.GetGenericArguments().First().GetGenericArguments().First() : 
                        context.ServiceMethod.ReturnType.GetGenericArguments().First();
                    
                    dynamic entities = Convert.ChangeType(obj, typeof(List<>).MakeGenericType(entityType));

                    for (var i = 0; i < entities.Count; i++)
                    {
                        var entity = entities[i];

                        uow.CacheEntity(entity);

                        // 使用跟踪的值取代从数据库中查询的值
                        entities[i] = uow.GetTrackedEntity(entity);
                    }

                    var returnType = context.IsAsync() ? context.ServiceMethod.ReturnType.GetGenericArguments().First() : context.ServiceMethod.ReturnType;
                    context.ReturnValue = context.IsAsync() ? _taskResultMethod.MakeGenericMethod(returnType).Invoke(null, new object[] { entities }) : entities;
                }
            }
        }
    }
}
