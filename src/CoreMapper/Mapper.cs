using CoreMapper;
using CoreMapper.Defaults;
using CoreMapper.Strategies;
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
        private IList<IMappingStrategy> MappingStrategies { get; }

        private IDictionary<TypeKey, Mapping> _maps;

        public void RegisterStrategy<TStrategy>()
             where TStrategy : class, IMappingStrategy, new()
        {
            RegisterStrategy(new TStrategy());
        }

        public void RegisterStrategy<TStrategy>(TStrategy mappingStrategy)
            where TStrategy : class, IMappingStrategy
        {
            if(MappingStrategies.Contains(mappingStrategy) == false)
            {
                MappingStrategies.Add(mappingStrategy);
            }
        }

        public void RegisterInterceptor<TInterceptor>()
            where TInterceptor : class, IInterceptor, new()
        {
            RegisterInterceptor(new TInterceptor());
        }

        public void RegisterInterceptor<TInterceptor>(TInterceptor interceptor = null)
            where TInterceptor : class, IInterceptor
        {
            if(Interceptors.Contains(interceptor) == false)
            {
                Interceptors.Add(interceptor);
            }
        }       

        public void Map(object source, object destination)
        {
            Mapping action = GetMap(source.GetType(), destination.GetType());

            action.Action(source, destination);
        }
        public TTargetType Map<TTargetType>(object source)
            where TTargetType : class
        {
            Mapping mapping = GetMap(source.GetType(), typeof(TTargetType));

            TTargetType target = (TTargetType)mapping.CreateTarget();

            Map(source, target);

            return target;
        }

        public IEnumerable<TTarget> Map<TTarget>(IEnumerable<object> sources)
            where TTarget : class
        {
            List<TTarget> result = new List<TTarget>();

            foreach(object source in sources)
            {
                Mapping mapping = GetMap(source.GetType(), typeof(TTarget));

                TTarget target = (TTarget)mapping.CreateTarget();

                Map(source, target);

                result.Add(target);
            }

            return result;
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
            
            //Expression typeFactory = Expression.Property(Expression.Constant(this), nameof(TypeFactory));

            UnaryExpression source_ = Expression.Convert(sourceExpression, sourceType);
            UnaryExpression target_ = Expression.Convert(destinationExpression, targetType);
            
            List<Expression> block = new List<Expression>();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                if(sourceProperty.Name == "Id")
                {

                }
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

            //create mapping method
            Expression<Action<object, object>> lambda = Expression.Lambda<Action<object, object>>(
                                                     Expression.Block(block),
                                                     sourceExpression,
                                                     destinationExpression
                                                     );

            //create target
            Expression<Func<object>> createTargetTypeExpr = Expression.Lambda<Func<object>>(Expression.New(targetType));

            return new Mapping()
            {
                Action = lambda.Compile(),
                CreateTarget = createTargetTypeExpr.Compile()
            };
        }

      
    }
}
