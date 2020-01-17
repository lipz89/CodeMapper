using System;
using System.Linq.Expressions;

namespace CodeMapper.Metas
{
    public interface IBindingConfig<TSource, TTarget>
    {
        IBindingConfig<TSource, TTarget> Bind<TProperty>(Expression<Func<TTarget, TProperty>> target, Expression<Func<TSource, TProperty>> source);
        IBindingConfig<TSource, TTarget> Ignore<TProperty>(Expression<Func<TTarget, TProperty>> expression);
    }
}
