using CoreMapper;
using CoreMapper.Defaults;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreMapper
{
    /// <summary>
    /// Mapper
    /// </summary>
    public class Mapper : IMapper
    {
        public static Mapper Default { get; private set; }

        static Mapper()
        {
            Default = new Mapper();
        }

        public Mapper()
        {
            _maps = new Dictionary<TypeKey, Mapping>();

            TypeFactory = new DefaultTypeFactory();
            Interceptors = new List<IInterceptor>();
            MappingStrategies = new List<IMappingStrategy>();
        }

        /// <summary>
        /// TypeFactory
        /// </summary>
        public ITypeFactory TypeFactory { get; set; }        

        /// <summary>
        /// Interceptors
        /// </summary>
        public IList<IInterceptor> Interceptors { get; }

        /// <summary>
        /// MappingStrategies
        /// </summary>
        public IList<IMappingStrategy> MappingStrategies { get; }

        //public void Map<TSource, TDestination>(TSource source, TDestination destination, IEntitySelector entitySelector = null)
        //{
        //    var action = GetMap(typeof(TSource), typeof(TDestination));

        //    action.Action(source, destination, entitySelector);
        //}

        private IDictionary<TypeKey, Mapping> _maps;
        
        public void Map(Object source, Object destination)
        {
            var action = GetMap(source.GetType(), destination.GetType());

            action.Action(source, destination);
            
            //action(source, destination);
        }

        protected Mapping GetMap(Type source, Type destination)
        {
            TypeKey typeKey = new TypeKey(source, destination);
            
            if (_maps.TryGetValue(typeKey, out Mapping value) == false)
            {
                value = BuildMap(source, destination);

                _maps.Add(typeKey, value);
            }

            return value;
        }

        protected Mapping BuildMap(Type sourceType, Type targetType)
        {
            PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x=> x.CanRead).ToArray();
            PropertyInfo[] destinationProperties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite).ToArray();

            ParameterExpression sourceExpression = Expression.Parameter(typeof(object), "source");
            ParameterExpression destinationExpression = Expression.Parameter(typeof(object), "target");
            
            Expression typeFactory = Expression.Property(Expression.Constant(this), nameof(TypeFactory));

            UnaryExpression source_ = Expression.Convert(sourceExpression, sourceType);
            UnaryExpression target_ = Expression.Convert(destinationExpression, targetType);
            
            List<Expression> block = new List<Expression>();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                Expression exprSourceProperty = Expression.Property(source_, sourceProperty);
                
                MappingContext context = new MappingContext(this, source_, sourceType, sourceProperty, exprSourceProperty, target_, targetType);

                foreach (IMappingStrategy strategy in MappingStrategies)
                {
                    Expression expr = strategy.Apply(context);

                    if (expr != null)
                    {
                        //skip all source null values
                        if (sourceProperty.PropertyType.IsValueType == false || sourceProperty.PropertyType.IsNullable())
                        {
                            expr = Expression.IfThen(Expression.NotEqual(exprSourceProperty, Expression.Constant(null)), expr);
                        }

                        //interceptors
                        foreach (IInterceptor interceptor in Interceptors)
                        {
                            if (interceptor.UseForType(sourceType))
                            {
                                expr = interceptor.SetValueToProperty(expr, source_, sourceProperty);
                            }
                        }

                        block.Add(expr);
                    }
                }
            }

            Expression<Action<object, object>> lambda = Expression.Lambda<Action<object, object>>(
                                                     Expression.Block(block),
                                                     sourceExpression,
                                                     destinationExpression
                                                     );
            
            Action<object, object> compiled = lambda.Compile();

            return new Mapping() { Action = compiled };
        }
    }
}
