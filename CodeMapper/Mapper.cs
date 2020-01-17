using CodeMapper.Commons;
using CodeMapper.Metas;
using System;
using System.Collections.Generic;

namespace CodeMapper
{
    /// <summary>
    /// 映射转换公共入口
    /// </summary>
    public static class Mapper
    {
        #region Bind & Config
        /// <summary>
        /// 使用默认规则映射类型
        /// </summary>
        /// <param name="source">源类型</param>
        /// <param name="target">目标类型</param>
        public static void Bind(Type source, Type target)
        {
            MapperUtil.Bind(source, target);
        }
        /// <summary>
        /// 使用BindingConfig映射类型
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="config">映射规则，没有规则则使用默认规则</param>
        public static void Bind<TSource, TTarget>(Action<IBindingConfig<TSource, TTarget>> config = null)
        {
            MapperUtil.Bind<TSource, TTarget>(config);
        }
        /// <summary>
        /// 使用自定义委托映射类型
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="mapper">自定义委托，不可空</param>
        public static void BindCustom<TSource, TTarget>(Func<TSource, TTarget> mapper)
        {
            MapperUtil.BindCustom<TSource, TTarget>(mapper);
        }
        /// <summary>
        /// 获取类型的成员映射规则
        /// </summary>
        /// <param name="source">源类型</param>
        /// <param name="target">目标类型</param>
        /// <returns></returns>
        public static List<MappingMember> GetMemberBinding(Type source, Type target)
        {
            return MapperUtil.GetMemberBinding(source, target);
        }
        /// <summary>
        /// 获取类型的成员映射规则
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <returns></returns>
        public static List<MappingMember> GetMemberBinding<TSource, TTarget>()
        {
            return MapperUtil.GetMemberBinding<TSource, TTarget>();
        }
        /// <summary>
        /// 全局配置
        /// </summary>
        /// <param name="config"></param>
        public static void Config(Action<IMapperConfig> config)
        {
            MapperUtil.Config(config);
        }

        #endregion

        #region Map
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <returns>返回转换结果</returns>
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
        /// <summary>
        /// 批量转换
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象集合</param>
        /// <returns>转换结果</returns>
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
