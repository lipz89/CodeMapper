namespace CodeMapper.Mappers
{
    internal abstract class BaseMapper : IMapper
    {
        public object Map(object source, object target, bool autoMapReferenceProperty)
        {
            if(autoMapReferenceProperty)
            {
                return MapCoreWithReferenceProperty(source, target);
            }
            else
            {
                return MapCore(source, target);
            }
        }
        protected abstract object MapCore(object source, object target);
        protected virtual object MapCoreWithReferenceProperty(object source, object target)
        {
            return MapCore(source, target);
        }
    }
}
