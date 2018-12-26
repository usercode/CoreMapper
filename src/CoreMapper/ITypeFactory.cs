using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper
{
    public interface ITypeFactory
    {
        T Create<T>() where T : class, new();
    }
}
