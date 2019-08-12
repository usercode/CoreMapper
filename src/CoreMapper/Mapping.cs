using CoreMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper
{
    public class Mapping
    {
        public Action<object, object> Action;

        public Func<object> CreateTarget;
    }
}
