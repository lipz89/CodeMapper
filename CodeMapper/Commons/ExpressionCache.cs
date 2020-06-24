using System;
using System.Linq.Expressions;

namespace CodeMapper.Commons
{
    internal static class ExpressionCache<TSource, TTarget>
    {
        private static readonly Cache<string, LambdaExpression> Cache = new Cache<string, LambdaExpression>();
        public static void Add(string targetName, LambdaExpression expression)
        {
            Cache.Add(targetName, expression);
        }

        public static TValue GetValue<TValue>(string name, TSource source)
        {
            LambdaExpression exp = Cache.Get(name);
            var dlgt = exp.Compile();
            var rst = dlgt.DynamicInvoke(source);
            return (TValue)rst;
        }
    }
}
