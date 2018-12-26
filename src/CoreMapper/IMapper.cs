using CoreMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper
{
    public interface IMapper
    {
        /// <summary>
        /// Map
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        void Map(Object source, Object destination);

        /// <summary>
        /// Interceptors
        /// </summary>
        IList<IInterceptor> Interceptors { get; }

        /// <summary>
        /// MappingStrategies
        /// </summary>
        IList<IMappingStrategy> MappingStrategies { get; }
    }        
}
