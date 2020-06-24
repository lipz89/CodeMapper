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
        internal bool IsStarted = false;
        private bool bindWhenNeed;
        private int maxDepth = 3;
        private MultiMatchHandle multiMatchHandle;
        private ReferencePropertyHandle referencePropertyHandle;
        private readonly List<MemberInfo> _ignoreMembers = new List<MemberInfo>();
        public static readonly Func<string, string, bool> DefaultNameMatching = (source, target) => string.Equals(source, target, StringComparison.Ordinal);

        public MapperConfig()
        {
            NameMatching = DefaultNameMatching;
            BindWhenNeed = true;
            MultiMatchHandle = MultiMatchHandle.First;
            ReferencePropertyHandle = ReferencePropertyHandle.Ignore;
        }

        public Func<string, string, bool> NameMatching { get; private set; }
        public bool BindWhenNeed
        {
            get { return bindWhenNeed; }
            set { if(!IsStarted) bindWhenNeed = value; }
        }
        public int MaxDepth
        {
            get { return maxDepth; }
            set { if(!IsStarted) maxDepth = value.Between(1, 10); }
        }
        public MultiMatchHandle MultiMatchHandle
        {
            get { return multiMatchHandle; }
            set { if(!IsStarted) this.multiMatchHandle = value; }
        }

        public ReferencePropertyHandle ReferencePropertyHandle
        {
            get { return referencePropertyHandle; }
            set { if(!IsStarted) this.referencePropertyHandle = value; }
        }

        public void GlobalIgnore<T>(Expression<Func<T, dynamic>> member)
        {
            if(IsStarted)
                return;
            var memberInfo = member.GetMemberInfo();
            if(memberInfo != null)
            {
                _ignoreMembers.Add(memberInfo);
            }
        }

        public void SetNameMatching(Func<string, string, bool> nameMatching)
        {
            if(IsStarted)
                return;
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

        public void SetObject2String(Func<object, string> action)
        {
            MapperUtil.Object2String = action ?? (_ => _.ToString());
        }
        public void SetLogger(Action<string> action)
        {
            MapperUtil.Logger = action ?? (_ => { });
        }
    }
}
