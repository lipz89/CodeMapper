using CodeMapper.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeMapper.Metas
{
    internal sealed class MappingMemberBuilder
    {
        private readonly IMapperConfig _config;

        public MappingMemberBuilder(IMapperConfig config)
        {
            _config = config;
        }

        public List<MappingMember> Build(TypePair typePair)
        {
            return ParseMappingTypes(typePair);
        }

        private static List<MemberInfo> GetPublicMembers(Type type)
        {
            return type.GetMembers(BindingFlags.Instance | BindingFlags.Public)
                       .Where(x => x.MemberType == MemberTypes.Property || x.MemberType == MemberTypes.Field)
                       .ToList();
        }

        private static List<MemberInfo> GetSourceMembers(Type sourceType)
        {
            var result = new List<MemberInfo>();

            List<MemberInfo> members = GetPublicMembers(sourceType);
            foreach(MemberInfo member in members)
            {
                if(member.MemberType == MemberTypes.Property)
                {
                    MethodInfo method = ((PropertyInfo)member).GetGetMethod();
                    if(method.IsNull())
                    {
                        continue;
                    }
                }
                result.Add(member);
            }
            return result;
        }

        private static List<MemberInfo> GetTargetMembers(Type targetType)
        {
            var result = new List<MemberInfo>();

            List<MemberInfo> members = GetPublicMembers(targetType);
            foreach(MemberInfo member in members)
            {
                if(member.MemberType == MemberTypes.Property)
                {
                    MethodInfo method = ((PropertyInfo)member).GetSetMethod();
                    if((method.IsNull() || method.GetParameters().Length != 1) && ((PropertyInfo)member).PropertyType.IsValueType)
                    {
                        continue;
                    }
                }
                result.Add(member);
            }
            return result;
        }

        private MemberInfo GetSourceName(List<MemberInfo> sourceMembers, MemberInfo targetMember, BindingConfig bindingConfig)
        {
            if(bindingConfig != null)
            {
                var sourceName = bindingConfig.GetBindField(targetMember.Name);
                if(sourceName != null)
                {
                    return sourceMembers.FirstOrDefault(x => x.Name.Equals(sourceName));
                }
            }
            var matchSourceMembers = sourceMembers.Where(x => _config.NameMatching(targetMember.Name, x.Name));
            if(matchSourceMembers.Count() <= 1)
            {
                return matchSourceMembers.FirstOrDefault();
            }
            switch(_config.MultiMatchHandle)
            {
                case MultiMatchHandle.Ignore:
                    return null;
                case MultiMatchHandle.Error:
                    throw new Exception($"找到多个匹配的成员。TargetMemberName:{targetMember.Name}");
                default:
                    return matchSourceMembers.FirstOrDefault();
            }
        }

        private bool IsIgnore(MemberInfo targetMember, BindingConfig bindingConfig)
        {
            if(bindingConfig != null)
            {
                var sourceName = bindingConfig.GetBindField(targetMember.Name);
                if(sourceName != null)
                {
                    return false;
                }
                if(bindingConfig.IsIgnoreTargetField(targetMember.Name))
                {
                    return true;
                }
            }
            if(_config.IsGlobalIgnore(targetMember))
            {
                return true;
            }
            return false;
        }


        private bool IsExpressionMapper(MemberInfo targetMember, BindingConfig bindingConfig)
        {
            if(bindingConfig != null)
            {
                if(bindingConfig.IsExpressionMapper(targetMember.Name))
                {
                    return true;
                }
            }
            return false;
        }

        private List<MappingMember> ParseMappingTypes(TypePair typePair)
        {
            var result = new List<MappingMember>();

            List<MemberInfo> sourceMembers = GetSourceMembers(typePair.Source);
            List<MemberInfo> targetMembers = GetTargetMembers(typePair.Target);

            BindingConfig bindingConfig = BindingConfig.Get(typePair);

            foreach(var member in targetMembers)
            {
                if(IsIgnore(member, bindingConfig))
                {
                    result.Add(MappingMember.Ignore(member));
                    continue;
                }

                if(IsExpressionMapper(member, bindingConfig))
                {
                    result.Add(MappingMember.Expreesion(member));
                    continue;
                }

                MemberInfo sourceMember = GetSourceName(sourceMembers, member, bindingConfig);
                if(sourceMember.IsNull())
                {
                    result.Add(MappingMember.Ignore(member));
                    continue;
                }
                result.Add(MappingMember.Mapper(member, sourceMember));
            }
            return result;
        }
    }
}
