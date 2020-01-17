using System;
using System.Linq.Expressions;

namespace CodeMapper.Metas
{
    /// <summary>
    /// 映射规则
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IBindingConfig<TSource, TTarget>
    {
        /// <summary>
        /// 使用成员
        /// </summary>
        /// <typeparam name="TProperty">成员类型</typeparam>
        /// <param name="target">目标成员</param>
        /// <param name="source">源，如果是成员，直接映射成员，如果不是成员，通过表达式映射</param>
        /// <returns></returns>
        IBindingConfig<TSource, TTarget> Bind<TProperty>(Expression<Func<TTarget, TProperty>> target, Expression<Func<TSource, TProperty>> source);
        /// <summary>
        /// 忽略成员
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IBindingConfig<TSource, TTarget> Ignore<TProperty>(Expression<Func<TTarget, TProperty>> expression);
    }
}
