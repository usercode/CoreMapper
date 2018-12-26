using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreMapper.Entities
{
    public interface IEntitySelector
    {
        TEntity GetEntity<TEntity, TEntityKey>(TEntityKey id) 
            where TEntity : class, IEntity<TEntityKey>
            where TEntityKey : struct, IEquatable<TEntityKey>;

        TEntity CreateEntity<TEntity>()
            where TEntity : class, IEntity;
    }
}
