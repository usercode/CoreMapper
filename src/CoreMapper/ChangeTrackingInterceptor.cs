using CoreMapper;
using ObjectChangeTracking.Abstractions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectChangeTracking.CoreMapper
{
    public class ChangeTrackingInterceptor : IInterceptor
    {
        public bool UseForType(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(ITrackableObject));
        }

        public Expression SetValueToProperty(Expression setValueExpression, Expression sourceProperty, PropertyInfo sourcePropertyInfo)
        {
            return Expression.IfThen(
                         Expression.Call(typeof(Helper).GetMethod(nameof(Helper.IsChangedProperty)), sourceProperty, Expression.Constant(sourcePropertyInfo.Name)),
                         setValueExpression);
        }
    }
}
