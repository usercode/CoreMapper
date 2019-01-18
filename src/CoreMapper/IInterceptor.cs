using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreMapper
{
    public interface IInterceptor
    {
        bool UseForType(Type type);

        Expression SetValueToProperty(Expression setValueExpression, Expression sourceProperty, PropertyInfo sourcePropertyInfo);

    }
}
