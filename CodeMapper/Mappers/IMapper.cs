using CodeMapper.Metas;

namespace CodeMapper.Mappers
{
    internal interface IMapper
    {
        object Map(object source, object target, ReferencePropertyHandle referencePropertyHandle, int depth);
    }
}