using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CoreMapper;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace CoreMapper.Strategies
{
    public class SameNameAndTypeStrategy : IMappingStrategy
    {
        public Expression Apply(MappingContext mappingContext)
        {
            try
            {
                if(mappingContext.SourceProperty.Name == "Id")
                {
                    return null;
                }

                //same name and type
                PropertyInfo targetProperty = mappingContext.TargetType
                                                                .GetProperties()
                                                                .Where(x=> x.CanWrite)
                                                                .FirstOrDefault(x => x.Name == mappingContext.SourceProperty.Name);

                if (targetProperty == null)
                {
                    return null;
                }

                MemberExpression targetPropertyExpression = Expression.Property(mappingContext.Target, targetProperty);

                //is collection?
                if (mappingContext.SourceProperty.PropertyType != typeof(String) 
                    && mappingContext.SourceProperty.PropertyType.GetInterfaces().Any(x => x == typeof(IEnumerable)))
                {
                    ParameterExpression pp = Expression.Variable(mappingContext.SourceProperty.PropertyType.GetGenericArguments()[0]);
                    ParameterExpression createTypeParameter = Expression.Variable(targetProperty.PropertyType.GetGenericArguments()[0]);

                    return mappingContext.SourcePropertyExpression.ForEach(pp, Expression.Block(
                        new[] { createTypeParameter },
                        Expression.Assign(createTypeParameter, Expression.New(targetProperty.PropertyType.GetGenericArguments()[0])),
                        Expression.Call(Expression.Constant(mappingContext.Mapper), mappingContext.Mapper.GetType().GetMethod(nameof(mappingContext.Mapper.Map), new[] { typeof(object), typeof(object) }), pp, createTypeParameter),
                        Expression.Call(targetPropertyExpression, typeof(ICollection<>).MakeGenericType(targetProperty.PropertyType.GetGenericArguments()[0]).GetMethod("Add"), createTypeParameter)));
                }
                else
                {
                    //source: nullable primitive?
                    if (mappingContext.SourceProperty.PropertyType.IsNullable()
                        && targetProperty.PropertyType.IsNullable() == false)
                    {
                        //null check if source null
                        return Expression.IfThen(
                            Expression.NotEqual(mappingContext.SourcePropertyExpression, Expression.Constant(null)),
                         Expression.Assign(targetPropertyExpression, Expression.Convert(mappingContext.SourcePropertyExpression, targetProperty.PropertyType)));
                    }
                    //target: nullable primitive?
                    else if (mappingContext.SourceProperty.PropertyType.IsNullable() == false
                        && targetProperty.PropertyType.IsNullable())
                    {
                        return Expression.Assign(targetPropertyExpression, Expression.Convert(mappingContext.SourcePropertyExpression, targetProperty.PropertyType));
                    }
                    //default property
                    else
                    {
                        if (targetProperty.PropertyType.IsAssignableFrom(mappingContext.SourceProperty.PropertyType))
                        {
                            return Expression.Assign(targetPropertyExpression, mappingContext.SourcePropertyExpression);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

       
    }
}
