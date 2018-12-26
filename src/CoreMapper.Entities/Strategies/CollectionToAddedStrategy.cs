using CoreMapper;
using ObjectChangeTracking.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectChangeTracking.CoreMapper.Strategies
{
    public class CollectionToAddedStrategy : IMappingStrategy
    {
        public Expression Apply(MappingContext mappingContext)
        {
            PropertyInfo targetProperty = mappingContext.TargetType
                                                        .GetProperties()
                                                        .FirstOrDefault(x => x.Name == mappingContext.SourceProperty.Name + "Added");

            if (targetProperty == null)
            {
                return null;
            }

            Expression targetPropertyExpression = Expression.Property(mappingContext.Target, targetProperty);

            //Expression addedProperty = Expression.Property(mappingContext.SourcePropertyExpression, "Added");

            ParameterExpression pp = Expression.Variable(mappingContext.SourceProperty.PropertyType.GetGenericArguments()[0]);
            ParameterExpression createTypeParameter = Expression.Variable(targetProperty.PropertyType.GetGenericArguments()[0]);

            Expression sourcePropertyAddedItems = 
                Expression.Property(
                    Expression.Convert(
                    mappingContext.SourcePropertyExpression, 
                    typeof(ITrackableCollection<>).MakeGenericType(mappingContext.SourceProperty.PropertyType.GetGenericArguments()[0])),
                "Added");

            return
                Expression.IfThen(
                        Expression.TypeIs(mappingContext.SourcePropertyExpression, typeof(ITrackableCollection)),
                    sourcePropertyAddedItems.ForEach(pp, Expression.Block(
                    new[] { createTypeParameter },
                    Expression.Assign(createTypeParameter, Expression.New(targetProperty.PropertyType.GetGenericArguments()[0])),
                    Expression.Call(Expression.Constant(mappingContext.Mapper), mappingContext.Mapper.GetType().GetMethod(nameof(IMapper.Map), new[] { typeof(object), typeof(object) }), pp, createTypeParameter),
                    Expression.Call(targetPropertyExpression, typeof(ICollection<>).MakeGenericType(targetProperty.PropertyType.GetGenericArguments()[0]).GetMethod("Add"), createTypeParameter))));

            
            //return Expression.Assign(targetPropertyExpression, addedProperty);
        }
    }
}
