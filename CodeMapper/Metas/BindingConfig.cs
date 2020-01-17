using CodeMapper.Commons;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CodeMapper.Metas
{
    internal class BindingConfig
    {
        private readonly Dictionary<string, string> _bindFields = new Dictionary<string, string>();
        private readonly HashSet<string> _expressionFields = new HashSet<string>();
        private readonly HashSet<string> _ignoreFields = new HashSet<string>();

        internal void BindExpression(string targetName)
        {
            _expressionFields.Add(targetName);
            if(_bindFields.ContainsKey(targetName))
            {
                _bindFields.Remove(targetName);
            }
            if(_ignoreFields.Contains(targetName))
            {
                _ignoreFields.Remove(targetName);
            }
        }

        internal void BindFields(string targetName, string sourceName)
        {
            _bindFields[targetName] = sourceName;
            if(_ignoreFields.Contains(targetName))
            {
                _ignoreFields.Remove(targetName);
            }
            if(_expressionFields.Contains(targetName))
            {
                _expressionFields.Remove(targetName);
            }
        }

        internal string GetBindField(string targetName)
        {
            string result;
            bool exsist = _bindFields.TryGetValue(targetName, out result);
            if(exsist)
            {
                return result;
            }
            return null;
        }

        internal bool IsExpressionMapper(string targetName)
        {
            return _expressionFields.Contains(targetName);
        }

        internal void IgnoreTargetField(string targetName)
        {
            _ignoreFields.Add(targetName);
            if(_bindFields.ContainsKey(targetName))
            {
                _bindFields.Remove(targetName);
            }
            if(_expressionFields.Contains(targetName))
            {
                _expressionFields.Remove(targetName);
            }
        }

        internal bool IsIgnoreTargetField(string targetName)
        {
            if(string.IsNullOrEmpty(targetName))
            {
                return true;
            }
            return _ignoreFields.Contains(targetName);
        }



        public static BindingConfig Get(TypePair typePair)
        {
            return Cache<TypePair, BindingConfig>.Get(typePair);
        }

        public static void Add(TypePair typePair, BindingConfig bindingConfig)
        {
            Cache<TypePair, BindingConfig>.Add(typePair, bindingConfig);
        }

        public static bool Has(TypePair typePair)
        {
            return Cache<TypePair, BindingConfig>.HasKey(typePair);
        }
        public static void Remove(TypePair typePair)
        {
            Cache<TypePair, BindingConfig>.Remove(typePair);
        }
    }
    internal sealed class BindingConfig<TSource, TTarget> : BindingConfig, IBindingConfig<TSource, TTarget>
    {
        private static string GetMemberName<T, TField>(Expression<Func<T, TField>> expression)
        {
            return expression.GetMemberInfo()?.Name;
        }
        public IBindingConfig<TSource, TTarget> Bind<TProperty>(Expression<Func<TTarget, TProperty>> target, Expression<Func<TSource, TProperty>> source)
        {
            string targetName = GetMemberName(target);
            if(targetName == null)
            {
                throw new ArgumentException("目标表达式不是一个公共属性或字段", nameof(target));
            }
            ExpressionCache<TTarget, TProperty>.Add(targetName, source);
            BindExpression(targetName);
            return this;
        }

        public IBindingConfig<TSource, TTarget> Ignore<TProperty>(Expression<Func<TTarget, TProperty>> expression)
        {
            string targetName = GetMemberName(expression);
            if(targetName == null)
            {
                throw new ArgumentException("目标表达式不是一个公共属性或字段", nameof(expression));
            }
            IgnoreTargetField(targetName);
            return this;
        }
    }
}
