using CoreMapper.Entities;
using CoreMapper.Strategies;
using Microsoft.Extensions.DependencyInjection;
using ObjectChangeTracking.CoreMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper.AspNetCore
{
    public static class ServiceExtensions
    {
        public static void AddMapper<TEntitySelector>(this IServiceCollection services)
            where TEntitySelector : class, IEntitySelector
        {
            services.AddScoped<TEntitySelector>();
            services.AddScoped<IMapper>(service =>
            {
                Mapper mapper = new Mapper();

                IEntitySelector entitySelector = service.GetService<TEntitySelector>();

                mapper.RegisterStrategy<SameNameAndTypeStrategy>();
                mapper.RegisterStrategy<EntityToIdStrategry>();
                mapper.RegisterStrategy(new IdToEntityStrategy(entitySelector));
                mapper.RegisterStrategy(new AddedToCollectionStrategy(entitySelector));
                mapper.RegisterStrategy(new RemovedToCollectionStrategy(entitySelector));
                mapper.RegisterStrategy<CollectionToAddedStrategy>();
                mapper.RegisterStrategy<CollectionToRemovedStrategy>();
                mapper.RegisterInterceptor<ChangeTrackingInterceptor>();

                return mapper;
            });
        }
    }
}
