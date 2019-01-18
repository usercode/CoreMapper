using CoreMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper
{
    /// <summary>
    /// IMapper
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Map
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        void Map(object source, object destination);

        /// <summary>
        /// RegisterStrategy
        /// </summary>
        /// <typeparam name="TStrategy"></typeparam>
        /// <param name="mappingStrategy"></param>
        void RegisterStrategy<TStrategy>()
           where TStrategy : class, IMappingStrategy, new();

        void RegisterStrategy<TStrategy>(TStrategy mappingStrategy)
           where TStrategy : class, IMappingStrategy;

        /// <summary>
        /// RegisterInterceptor
        /// </summary>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="interceptor"></param>
        void RegisterInterceptor<TInterceptor>()
            where TInterceptor : class, IInterceptor, new();

        void RegisterInterceptor<TInterceptor>(TInterceptor interceptor)
            where TInterceptor : class, IInterceptor;
    }        
}
