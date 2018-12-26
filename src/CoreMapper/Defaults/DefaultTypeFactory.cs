using CoreMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper.Defaults
{
    class DefaultTypeFactory : ITypeFactory
    {
        public T Create<T>()
            where T : class, new()
        {
            return new T();
        }
    }
}
