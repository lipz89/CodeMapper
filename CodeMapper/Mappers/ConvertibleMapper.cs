using CodeMapper.Commons;
using CodeMapper.Metas;
using System;

namespace CodeMapper.Mappers
{
    internal class ConvertibleMapper : BaseMapper
    {
        private Func<object, object> innerMapper;
        public ConvertibleMapper(TypePair pair)
        {
            innerMapper = Converter.Get(pair);
        }
        protected override object MapCore(object source, object target)
        {
            return innerMapper(source);
        }
    }
}
