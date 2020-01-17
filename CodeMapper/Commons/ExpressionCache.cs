using System.Linq.Expressions;

namespace CodeMapper.Commons
{
    internal static class ExpressionCache<TSource, TTarget>
    {
        public static void Add(string targetName, LambdaExpression expression)
        {
            Cache<string, LambdaExpression>.Add(targetName, expression);
        }

        public static TValue GetValue<TValue>(string name, TSource source)
        {
            LambdaExpression exp = Cache<string, LambdaExpression>.Get(name);
            var dlgt = exp.Compile();
            var rst = dlgt.DynamicInvoke(source);
            return (TValue)rst;
        }
    }
}
