namespace CodeMapper.Mappers
{
    internal interface IMapper
    {
        object Map(object source, object target,bool autoMapReferenceProperty);
    }
}