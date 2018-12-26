using System;

namespace CoreMapper.Entities
{
    public interface IMapperEntity<TEntityKey>
        where TEntityKey : struct
    {
        TEntityKey Id { get; }
    }
}
