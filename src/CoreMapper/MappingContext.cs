using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreMapper
{
    public class MappingContext
    {
        public MappingContext(
            IMapper mapper,
            Expression source, 
            Type sourceType,
            PropertyInfo sourceProperty,
            Expression sourcePropertyExpression,
            Expression target,
            Type targetType)
        {
            Mapper = mapper;
            Source = source;
            SourceType = sourceType;
            SourceProperty = sourceProperty;
            SourcePropertyExpression = sourcePropertyExpression;
            Target = target;
            TargetType = targetType;
        }

        public IMapper Mapper { get; }
        public Expression Source { get; }
        public PropertyInfo SourceProperty { get; }
        public Expression SourcePropertyExpression { get; }
        public Type SourceType { get; }

        public Expression Target { get; }
        public Type TargetType { get; }
    }
}
