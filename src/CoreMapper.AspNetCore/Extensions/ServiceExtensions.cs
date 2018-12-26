using CoreMapper.Entities;
using CoreMapper.Entities.Strategies;
using CoreMapper.Strategies;
using Microsoft.Extensions.DependencyInjection;
using ObjectChangeTracking.CoreMapper;
using ObjectChangeTracking.CoreMapper.Strategies;
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

                mapper.MappingStrategies.Add(new SameNameAndTypeStrategy());
                mapper.MappingStrategies.Add(new EntityToIdStrategry());
                mapper.MappingStrategies.Add(new IdToEntityStrategy(entitySelector));
                mapper.MappingStrategies.Add(new AddedToCollectionStrategy(entitySelector));
                mapper.MappingStrategies.Add(new RemovedToCollectionStrategy(entitySelector));
                mapper.MappingStrategies.Add(new CollectionToAddedStrategy());
                mapper.MappingStrategies.Add(new CollectionToRemovedStrategy());
                mapper.Interceptors.Add(new ChangeTrackingInterceptor());

                return mapper;
            });
        }
    }
}
