using AspectCore.DynamicProxy;
using AspectCore.DynamicProxy.Parameters;
using Repository.Extensions;
using Repository.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Attributes
{
    public class CheckEntityAttribute : AbstractInterceptorAttribute
    {
        private readonly bool _newGuid;

        public CheckEntityAttribute(bool newGuid = false)
        {
            _newGuid = newGuid;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            context.GetParameters().ToList().ForEach(parameter =>
            {
                var paramType = parameter.Type;
                
                if (paramType.IsSubclassOf(typeof(BaseEntity)))
                {
                    var error = $"Unable to track an entity of type '{paramType.Name}' because its primary key is null.";
                    var paramValue = parameter.Value;

                    if (paramValue == null)
                        throw new ArgumentNullException(parameter.Name);

                    var idPropertyInfos = paramType.GetIdPropertyInfos();

                    foreach (var pi in idPropertyInfos)
                    {
                        var value = pi.GetValue(paramValue);

                        if (value == null)
                            throw new ApplicationException(error);
                        
                        if (pi.PropertyType == typeof(Guid) && (Guid)value == Guid.Empty)
                        {
                            if (_newGuid)
                                pi.SetValue(paramValue, Guid.NewGuid());
                            else
                                throw new ApplicationException(error);
                        }
                    }
                }
            });

            await next(context);
        }
    }
}
