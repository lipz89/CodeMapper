using CodeMapper.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeMapper.Metas
{
    internal sealed class MapperConfig : IMapperConfig
    {
        private readonly List<MemberInfo> _ignoreMembers = new List<MemberInfo>();
        public static readonly Func<string, string, bool> DefaultNameMatching = (source, target) => string.Equals(source, target, StringComparison.Ordinal);

        public MapperConfig()
        {
            NameMatching = DefaultNameMatching;
            BindWhenNeed = false;
            MultiMatchHandle = MultiMatchHandle.First;
        }

        public Func<string, string, bool> NameMatching { get; private set; }
        public bool BindWhenNeed { get; set; } = false;
        public MultiMatchHandle MultiMatchHandle { get; set; }

        public void GlobalIgnore<T>(Expression<Func<T, dynamic>> member)
        {
            var memberInfo = member.GetMemberInfo();
            if(memberInfo != null)
            {
                _ignoreMembers.Add(memberInfo);
            }
        }

        public void SetNameMatching(Func<string, string, bool> nameMatching)
        {
            NameMatching = nameMatching ?? DefaultNameMatching;
        }

        public bool IsGlobalIgnore(MemberInfo member)
        {
            if(member == null)
            {
                return true;
            }
            return _ignoreMembers.Any(x => x.Name == member.Name && x.GetMemberType() == member.GetMemberType());
        }
        public void Clear()
        {
            NameMatching = DefaultNameMatching;
            BindWhenNeed = false;
            _ignoreMembers.Clear();
        }
    }

    public enum MultiMatchHandle
    {
        Ignore,
        First,
        Error,
    }
}
