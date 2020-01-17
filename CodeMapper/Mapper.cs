using CodeMapper.Commons;
using CodeMapper.Metas;
using System;
using System.Collections.Generic;

namespace CodeMapper
{
    public static class Mapper
    {
        #region Bind & Config
        public static void Bind(Type source, Type target)
        {
            MapperUtil.Bind(source, target);
        }
        public static void Bind<TSource, TTarget>(Action<IBindingConfig<TSource, TTarget>> config = null)
        {
            MapperUtil.Bind<TSource, TTarget>(config);
        }
        public static void BindCustom<TSource, TTarget>(Func<TSource, TTarget> mapper)
        {
            MapperUtil.BindCustom<TSource, TTarget>(mapper);
        }
        public static List<MappingMember> GetMemberBinding(Type source, Type target)
        {
            return MapperUtil.GetMemberBinding(source, target);
        }
        public static List<MappingMember> GetMemberBinding<TSource, TTarget>()
        {
            return MapperUtil.GetMemberBinding<TSource, TTarget>();
        }
        public static void Config(Action<IMapperConfig> config)
        {
            MapperUtil.Config(config);
        }

        #endregion

        #region Map
        public static TTarget Map<TSource, TTarget>(TSource source, TTarget target = default)
        {
            try
            {
                return MapperUtil.MapCore<TSource, TTarget>(source, target);
            }
            finally
            {
                MapperCache.Clear();
            }
        }
        public static IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> source)
        {
            try
            {
                return MapperUtil.MapCores<TSource, TTarget>(source);
            }
            finally
            {
                MapperCache.Clear();
            }
        }
        #endregion
    }
}
