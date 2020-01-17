using CodeMapper.Commons;
using System.Reflection;

namespace CodeMapper.Metas
{
    /// <summary>
    /// 成员映射规则
    /// </summary>
    public sealed class MappingMember
    {
        private MappingMember()
        {

        }
        internal static MappingMember Mapper(MemberInfo target, MemberInfo source)
        {
            return Mapper(target, source, new TypePair(source.GetMemberType(), target.GetMemberType()));
        }
        internal static MappingMember Ignore(MemberInfo target)
        {
            return new MappingMember
            {
                Target = target,
                Ignored = true
            };
        }

        internal static MappingMember Mapper(MemberInfo target, MemberInfo source, TypePair typePair)
        {
            return new MappingMember
            {
                Source = source,
                Target = target,
                TypePair = typePair,
                IsMemberMapping = true
            };
        }
        internal static MappingMember Expreesion(MemberInfo target)
        {
            return new MappingMember
            {
                Target = target,
                IsExpressionMapping = true
            };
        }
        /// <summary>
        /// 成员被忽略
        /// </summary>
        public bool Ignored { get; private set; }
        /// <summary>
        /// 成员映射
        /// </summary>
        public bool IsMemberMapping { get; private set; }
        /// <summary>
        /// 表达式映射
        /// </summary>
        public bool IsExpressionMapping { get; private set; }
        /// <summary>
        /// 源成员，只有成员映射时才有
        /// </summary>
        public MemberInfo Source { get; private set; }
        /// <summary>
        /// 目标成员
        /// </summary>
        public MemberInfo Target { get; private set; }
        internal TypePair TypePair { get; private set; }

        public override string ToString()
        {
            if(Ignored)
            {
                return $"[{Target.Name}] is ignored";
            }
            else if(IsExpressionMapping)
            {
                return $"[{Target.Name}] from a expression";
            }
            else
            {
                return $"[{Target.Name}] from [{Source.Name}]";
            }
        }
    }
}
