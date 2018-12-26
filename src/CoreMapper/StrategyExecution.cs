using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CoreMapper
{
    public class StrategyExecution
    {
        public StrategyExecution(Func<Expression> action)
        {
            _action = action;
        }

        private Func<Expression> _action { get; }

        public Expression Execute()
        {
            return _action();
        }
    }
}
