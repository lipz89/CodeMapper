using CodeMapper.Builders;
using CodeMapper.Commons;
using CodeMapper.Metas;
using System;
using System.Collections.Generic;

namespace CodeMapper.Mappers
{
    internal class ObjectMapper : BaseMapper
    {
        private readonly Func<object, object, int, object> innerMapper;
        private readonly Action<object, object, int> innerMapperRef;
        public ObjectMapper(TypePair pair)
        {
            innerMapper = ExpressionBuilder.MapperCache.Get(pair);
            innerMapperRef = ExpressionBuilder.MapperRefCache.Get(pair);
        }
        protected override object MapCore(object source, object target, int depth)
        {
            var result = innerMapper(source, target, depth);
            innerMapperRef?.Invoke(source, result, depth);
            return result;
        }
        protected override object MapCoreLoop(object source, object target)
        {
            var key = GetKey(source);
            object rst;
            var flag = MapperCache.TryGet(key, out rst);
            if(flag)
            {
                return rst;
            }
            var result = innerMapper(source, target, 0);
            MapperCache.Set(key, result);
            innerMapperRef?.Invoke(source, result, 0);
            return result;
        }

        public static long GetKey(object instance)
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
