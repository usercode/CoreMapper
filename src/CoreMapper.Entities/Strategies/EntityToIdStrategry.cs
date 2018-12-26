using CoreMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreMapper.Entities.Strategies
{
    public class EntityToIdStrategry : IMappingStrategy
    {
        public EntityToIdStrategry()
        {
        }

        public Expression Apply(MappingContext mappingContext)
        {
            PropertyInfo targetProperty = mappingContext.TargetType.GetProperties().FirstOrDefault(x => x.Name == mappingContext.SourceProperty.Name + "Id");

            if (targetProperty == null)
            {
                return null;
            }

            MemberExpression targetPropertyExpression = Expression.Property(mappingContext.Target, targetProperty);

            return Expression.Assign(
                                Expression.Property(mappingContext.Target, targetProperty), 
                                Expression.Convert(Expression.Property(mappingContext.SourcePropertyExpression, "Id"), targetProperty.PropertyType));
        }
    }
}
