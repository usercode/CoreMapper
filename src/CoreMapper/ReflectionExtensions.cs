using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CoreMapper
{
    public static class ReflectionExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type GetNullable(this Type type)
        {
            Type subType = Nullable.GetUnderlyingType(type);

            if (subType != null)
            {
                return subType;
            }

            throw new Exception("No nullable type");
        }

        public static Type GetNullableOrSelf(this Type type)
        {
            Type subType = Nullable.GetUnderlyingType(type);

            if(subType != null)
            {
                return subType;
            }

            return type;
        }

        public static Expression ForEach(this Expression enumerable, ParameterExpression loopVar, Expression loopContent)
        {
            var elementType = loopVar.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(enumerable, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);
            var enumeratorDispose = Expression.Call(enumeratorVar, typeof(IDisposable).GetMethod("Dispose"));

            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label("LoopBreak");

            var trueConstant = Expression.Constant(true);

            var loop =
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, trueConstant),
                        Expression.Block(
                            new[] { loopVar },
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                            loopContent),
                        Expression.Break(breakLabel)),
                    breakLabel);

            var tryFinally =
                Expression.TryFinally(
                    loop,
                    enumeratorDispose);

            var body =
                Expression.Block(
                    new[] { enumeratorVar },
                    enumeratorAssign,
                    tryFinally);

            return body;
        }
    }
}
