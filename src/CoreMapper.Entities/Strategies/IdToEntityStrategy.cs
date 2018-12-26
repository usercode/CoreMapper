using CoreMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreMapper.Entities.Strategies
{
    public class IdToEntityStrategy : IMappingStrategy
    {
        public IdToEntityStrategy(IEntitySelector entitySelector)
        {
            EntitySelector = entitySelector;
        }

        public IEntitySelector EntitySelector { get; }

        public Expression Apply(MappingContext mappingContext)
        {
            if(mappingContext.SourceProperty.Name.EndsWith("Id") == false)
            {
                return null;
            }

            String targetName = mappingContext.SourceProperty.Name.Substring(0, mappingContext.SourceProperty.Name.Length - "Id".Length);

            PropertyInfo targetProperty = mappingContext.TargetType
                                                            .GetProperties()
                                                            .FirstOrDefault(x => x.Name == targetName);

            if (targetProperty == null)
            {
                return null;
            }
            
            MethodCallExpression resultCall = Expression.Call(
                                       Expression.Constant(EntitySelector),
                                       typeof(IEntitySelector).GetMethod(nameof(IEntitySelector.GetEntity)).MakeGenericMethod(targetProperty.PropertyType, mappingContext.SourceProperty.PropertyType.GetNullableOrSelf()),
                                       Expression.Convert(mappingContext.SourcePropertyExpression, mappingContext.SourceProperty.PropertyType.GetNullableOrSelf()));

            return Expression.Assign(
                                    Expression.Property(mappingContext.Target, targetProperty), 
                                    resultCall);
        }
    }
}
