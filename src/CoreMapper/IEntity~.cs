using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper.Entities
{
    public interface IEntity<TKey> : IEntity
        where TKey : struct
    {
        TKey Id { get; }
    }
}
