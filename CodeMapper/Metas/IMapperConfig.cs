using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeMapper.Metas
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public interface IMapperConfig
    {
        /// <summary>
        /// 成员名称匹配规则
        /// </summary>
        Func<string, string, bool> NameMatching { get; }
        /// <summary>
        /// 发现未映射的类型，true自动映射,false不自动映射
        /// </summary>
        bool BindWhenNeed { get; set; }
        /// <summary>
        /// 引用类型属性的处理方式，默认Ignore，执行为Depth时需要指定MaxDepth
        /// </summary>
        ReferencePropertyHandle ReferencePropertyHandle { get; set; }
        /// <summary>
        /// 引用属性映射深度，介于1-10之间，默认3
        /// </summary>
        int MaxDepth { get; set; }
        /// <summary>
        /// 发现多个匹配的映射源成员时的处理方式
        /// </summary>
        MultiMatchHandle MultiMatchHandle { get; set; }
        /// <summary>
        /// 设置成员名称匹配规则，如果为空，使用完全匹配规则
        /// </summary>
        /// <param name="nameMatching"></param>
        void SetNameMatching(Func<string, string, bool> nameMatching);

        void SetObject2String(Func<object, string> action);

        void SetLogger(Action<string> action);
        /// <summary>
        /// 全局忽略成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        void GlobalIgnore<T>(Expression<Func<T, dynamic>> member);
        /// <summary>
        /// 是否全局忽略
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        bool IsGlobalIgnore(MemberInfo member);
    }
}
