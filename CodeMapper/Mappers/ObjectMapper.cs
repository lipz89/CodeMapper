using CodeMapper.Commons;
using CodeMapper.Metas;
using System;
using System.Collections.Generic;

namespace CodeMapper.Mappers
{
    internal class ObjectMapper : BaseMapper
    {
        private readonly Func<object, object, object> innerMapper;
        private readonly Action<object, object> innerMapperRef;
        public ObjectMapper(TypePair pair)
        {
            innerMapper = Cache<TypePair, Func<object, object, object>>.Get(pair);
            innerMapperRef = Cache<TypePair, Action<object, object>>.Get(pair);
        }
        protected override object MapCore(object source, object target)
        {
            var key = GetKey(source);
            object rst;
            var flag = MapperCache.TryGet(key, out rst);
            if(flag)
            {
                return rst;
            }
            var result = innerMapper(source, target);
            MapperCache.Set(key, result);
            innerMapperRef?.Invoke(source, result);
            return result;
        }

        private static long GetKey(object instance)
        {
            if(instance == null)
                return 0;
            Type type = instance.GetType();
            long thash = (long)GetTypeFullName(type).GetHashCode() << 32;
            var objHash = (long)instance.GetHashCode();
            return thash | objHash;
        }

        private static string GetTypeFullName(Type type)
        {
            var str = type.FullName;
            if(type.IsGenericType)
            {
                var gts = type.GetGenericArguments();
                var gtstrs = new List<string>();
                foreach(var gt in gts)
                {
                    gtstrs.Add(GetTypeFullName(gt));
                }
                str += "<" + string.Join(",", gtstrs) + ">";
            }
            return str;
        }
    }
}
