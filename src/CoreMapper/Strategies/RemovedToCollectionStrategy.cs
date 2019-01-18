using CoreMapper;
using CoreMapper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreMapper.Strategies
{
    public class RemovedToCollectionStrategy : IMappingStrategy
    {
        public RemovedToCollectionStrategy(IEntitySelector entitySelector)
        {
            EntitySelector = entitySelector;
        }

        public IEntitySelector EntitySelector { get; }
        

        
        public Expression Apply(MappingContext mappingContext)
        {
            if (mappingContext.SourceProperty.Name.Length <= "Removed".Length)
            {
                return null;
            }

            string targetName = mappingContext.SourceProperty.Name.Substring(0, mappingContext.SourceProperty.Name.Length - "Removed".Length);

            PropertyInfo targetProperty = mappingContext.TargetType
                                                        .GetProperties()
                                                        .FirstOrDefault(x => x.Name == targetName);

            if (targetProperty == null)
            {
                return null;
            }

            Expression targetPropertyExpression = Expression.Property(mappingContext.Target, targetProperty);

            //Expression addedProperty = Expression.Property(mappingContext.SourcePropertyExpression, "Added");

            ParameterExpression pp = Expression.Variable(mappingContext.SourceProperty.PropertyType.GetGenericArguments()[0]);

            ParameterExpression createTypeParameter = Expression.Variable(targetProperty.PropertyType.GetGenericArguments()[0]);
            
            return mappingContext.SourcePropertyExpression.ForEach(pp, Expression.Block(
                new[] { createTypeParameter },
                Expression.Assign(createTypeParameter, Expression.New(targetProperty.PropertyType.GetGenericArguments()[0])),
                Expression.Call(Expression.Constant(mappingContext.Mapper), mappingContext.Mapper.GetType().GetMethod(nameof(IMapper.Map), new[] { typeof(object), typeof(object) }), pp, createTypeParameter),
                Expression.Call(targetPropertyExpression, typeof(ICollection<>).MakeGenericType(targetProperty.PropertyType.GetGenericArguments()[0]).GetMethod("Remove"), createTypeParameter)));
            
        }
    }
}
