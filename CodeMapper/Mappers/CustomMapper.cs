using CodeMapper.Commons;
using CodeMapper.Metas;
using System;
using System.Collections.Generic;

namespace CodeMapper.Mappers
{
    internal class CustomMapper : BaseMapper
    {
        private Func<object, object> innerMapper;
        public CustomMapper(TypePair pair)
        {
            innerMapper = CustomMapper.Get(pair);
        }
        protected override object MapCore(object source, object target, int depth)
        {
            return innerMapper(source);
        }

        internal static void Add(TypePair pair, Func<object, object> mapper)
        {
            Converter.Cache.Add(pair, mapper);
        }

        internal static Func<object, object> Get(TypePair pair)
        {
            return Converter.Cache.Get(pair);
        }

        internal static bool Has(TypePair pair)
        {
            return Converter.Cache.HasKey(pair);
        }
        internal static void Remove(TypePair pair)
        {
            Converter.Cache.Remove(pair);
        }
    }
}
