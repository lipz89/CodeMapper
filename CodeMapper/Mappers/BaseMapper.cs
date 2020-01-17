namespace CodeMapper.Mappers
{
    public abstract class BaseMapper : IMapper
    {
        public object Map(object source, object target)
        {
            return MapCore(source, target);
        }
        protected abstract object MapCore(object source, object target);
        public virtual void MapObjects(object source, object target)
        {

        }
    }
    //public abstract class Mapper<TSource, TTarget> : Mapper, IMapper<TSource, TTarget>
    //{
    //    public TTarget Map(TSource source, TTarget target)
    //    {
    //        return MapCore(source, target);
    //    }

    //    protected abstract TTarget MapCore(TSource source, TTarget target);

    //    public virtual void MapObjects(TSource source, TTarget target)
    //    {
    //    }

    //    protected override object MapCore(object source, object target)
    //    {
    //        return MapCore((TSource)source, (TTarget)target);
    //    }

    //    public override void MapObjects(object source, object target)
    //    {
    //        MapObjects((TSource)source, (TTarget)target);
    //    }
    //}
}
