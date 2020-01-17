using CodeMapper.Commons;
using CodeMapper.Metas;
using System;

namespace CodeMapper.Mappers
{
    internal class CustomMapper : BaseMapper
    {
        private Func<object, object> innerMapper;
        public CustomMapper(TypePair pair)
        {
            innerMapper = CustomMapper.Get(pair);
        }
        protected override object MapCore(object source, object target)
        {
            return innerMapper(source);
        }

        internal static void Add(TypePair pair, Func<object, object> mapper)
        {
            Cache<TypePair, Func<object, object>>.Add(pair, mapper);
        }

        internal static Func<object, object> Get(TypePair pair)
        {
            return Cache<TypePair, Func<object, object>>.Get(pair);
        }

        internal static bool Has(TypePair pair)
        {
            return Cache<TypePair, Func<object, object>>.HasKey(pair);
        }
        internal static void Remove(TypePair pair)
        {
            Cache<TypePair, Func<object, object>>.Remove(pair);
        }
    }
}
