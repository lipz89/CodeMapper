namespace CodeMapper.Mappers
{
    internal interface IMapper
    {
        object Map(object source, object target);
    }
    //public interface IMapper<TSource, TTarget> : IMapper
    //{
    //    TTarget Map(TSource source, TTarget target);
    //    void MapObjects(TSource source, TTarget target);
    //}
}