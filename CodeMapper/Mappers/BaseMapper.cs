using CodeMapper.Metas;

namespace CodeMapper.Mappers
{
    internal abstract class BaseMapper : IMapper
    {
        public object Map(object source, object target, ReferencePropertyHandle referencePropertyHandle, int depth)
        {
            if(referencePropertyHandle == ReferencePropertyHandle.Loop)
            {
                return MapCoreLoop(source, target);
            }
            else
            {
                return MapCore(source, target, depth);
            }
        }
        protected abstract object MapCore(object source, object target, int depth);
        protected virtual object MapCoreLoop(object source, object target)
        {
            return MapCore(source, target, 0);
        }
    }
}
