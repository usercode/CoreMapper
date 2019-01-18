using CoreMapper;
using ObjectChangeTracking.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreMapper.Strategies
{
    public class CollectionToRemovedStrategy : IMappingStrategy
    {
        public Expression Apply(MappingContext mappingContext)
        {
            PropertyInfo targetProperty = mappingContext.TargetType
                                                        .GetProperties()
                                                        .FirstOrDefault(x => x.Name == mappingContext.SourceProperty.Name + "Removed");

            if (targetProperty == null)
            {
                return null;
            }

            Expression targetPropertyExpression = Expression.Property(mappingContext.Target, targetProperty);

            ParameterExpression pp = Expression.Variable(mappingContext.SourceProperty.PropertyType.GetGenericArguments()[0]);
            ParameterExpression createTypeParameter = Expression.Variable(targetProperty.PropertyType.GetGenericArguments()[0]);

            Expression sourcePropertyAddedItems =
                Expression.Property(
                    Expression.Convert(
                    mappingContext.SourcePropertyExpression,
                    typeof(ITrackableCollection<>).MakeGenericType(mappingContext.SourceProperty.PropertyType.GetGenericArguments()[0])),
                "Removed");

            return
                Expression.IfThen(
                        Expression.TypeIs(mappingContext.SourcePropertyExpression, typeof(ITrackableCollection)),
                    sourcePropertyAddedItems.ForEach(pp, Expression.Block(
                   Expression.Call(targetPropertyExpression, typeof(ICollection<>).MakeGenericType(targetProperty.PropertyType.GetGenericArguments()[0]).GetMethod("Add"), Expression.Property(pp, "Id")))));


            //return Expression.Assign(targetPropertyExpression, addedProperty);
        }
    }
}
