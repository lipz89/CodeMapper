using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeMapper.Metas
{
    public interface IMapperConfig
    {
        Func<string, string, bool> NameMatching { get; }
        bool BindWhenNeed { get; set; }
        MultiMatchHandle MultiMatchHandle { get; set; }
        void SetNameMatching(Func<string, string, bool> nameMatching);
        void GlobalIgnore<T>(Expression<Func<T, dynamic>> member);
        bool IsGlobalIgnore(MemberInfo member);
        void Clear();
    }
}
