using CoreMapper;
using CoreMapper.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper.Tests
{
    class EntitySelector : IEntitySelector
    {
        public TEntity GetEntity<TEntity, TEntityKey>(TEntityKey id)
            where TEntity : class, IEntity<TEntityKey>
            where TEntityKey : struct, IEquatable<TEntityKey>
        {
            var entity = (TEntity)Activator.CreateInstance<TEntity>();
            //entity.Id = id;

            return entity;
        }

        TEntity IEntitySelector.CreateEntity<TEntity>()
        {
            return Activator.CreateInstance<TEntity>();
        }
    }
}
