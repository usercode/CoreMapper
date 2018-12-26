using CoreMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreMapper
{
    public interface IMappingStrategy
    {
        Expression Apply(MappingContext mappingContext);

    }
}
