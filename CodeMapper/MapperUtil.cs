using CodeMapper.Builders;
using CodeMapper.Commons;
using CodeMapper.Mappers;
using CodeMapper.Metas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMapper
{
    internal static class MapperUtil
    {
        private static readonly MapperConfig mapperConfig = new MapperConfig();
        private static readonly MapperBuilder targetMapperBuilder = MapperBuilder.GetMapperBuilder(mapperConfig);
        private static readonly Cache<TypePair, IMapper> Cache = new Cache<TypePair, IMapper>();

        internal static Func<object, string> Object2String { get; set; } = _ => _.ToString();

        internal static Action<string> Logger { get; set; } = _ => { };

        internal static IMapper GetMapper(TypePair pair)
        {
            return Cache.GetOrAdd(pair, () => GetMapperCore(pair));
        }

        private static IMapper GetMapperCore(TypePair pair)
        {
            if(CustomMapper.Has(pair))
            {
                return new CustomMapper(pair);
            }
            if(Converter.IsConvertibleType(pair))
            {
                return new ConvertibleMapper(pair);
            }
            if(!BindingConfig.Has(pair))
            {
                if(mapperConfig.BindWhenNeed)
                {
                    Bind(pair);
                }
                else
                {
                    throw new Exception($"没有绑定映射 {pair}");
                }
            }
            return new ObjectMapper(pair);
        }

        #region Bind & Config
        internal static void Bind(TypePair typePair)
        {
            BindingConfig.Add(typePair, null);
            CustomMapper.Remove(typePair);
            targetMapperBuilder.Build(typePair);
        }

        public static void Bind(Type source, Type target)
        {
            TypePair typePair = TypePair.Create(source, target);
            Bind(typePair);
        }

        public static void Bind<TSource, TTarget>(Action<IBindingConfig<TSource, TTarget>> config = null)
        {
            TypePair typePair = TypePair.Create<TSource, TTarget>();

            if(config == null)
            {
                Bind(typePair);
            }
            else
            {
                var bindingConfig = new BindingConfig<TSource, TTarget>();
                config(bindingConfig);
                BindingConfig.Add(typePair, bindingConfig);
                CustomMapper.Remove(typePair);
                targetMapperBuilder.Build(typePair);
            }
        }

        public static void BindCustom<TSource, TTarget>(Func<TSource, TTarget> mapper)
        {
            TypePair typePair = TypePair.Create<TSource, TTarget>();
            if(mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            Func<object, object> innerMapper = x => mapper((TSource)x);
            CustomMapper.Add(typePair, innerMapper);
        }

        public static List<MappingMember> GetMemberBinding(Type source, Type target)
        {
            TypePair typePair = TypePair.Create(source, target);
            var result = targetMapperBuilder.MemberBuilder.Build(typePair);
            return result;
        }

        public static List<MappingMember> GetMemberBinding<TSource, TTarget>()
        {
            return GetMemberBinding(typeof(TSource), typeof(TTarget));
        }

        public static bool BindingExists(Type source, Type target)
        {
            TypePair typePair = TypePair.Create(source, target);

            return BindingConfig.Has(typePair);
        }

        /// <summary>
        ///     Find out if a binding exists for Source to Target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <returns></returns>
        public static bool BindingExists<TSource, TTarget>()
        {
            return BindingExists(typeof(TSource), typeof(TTarget));
        }

        public static void Config(Action<IMapperConfig> config)
        {
            config(mapperConfig);
            mapperConfig.IsStarted = true;
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
                MapperUtil.PreMap(source);
                target = MapperUtil.MapCore<TSource, TTarget>(source, target, 0);
                MapperUtil.PostMap(target);
                return target;
            }
            catch(Exception ex)
            {
                MapperUtil.OnError(ex);
                throw;
            }
            finally
            {
                if(mapperConfig.ReferencePropertyHandle == ReferencePropertyHandle.Loop)
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
                MapperUtil.PreMap(source);
                var target = MapperUtil.MapCores<TSource, TTarget>(source, 0);
                MapperUtil.PostMap(target);
                return target;
            }
            catch(Exception ex)
            {
                MapperUtil.OnError(ex);
                throw;
            }
            finally
            {
                if(mapperConfig.ReferencePropertyHandle == ReferencePropertyHandle.Loop)
                    MapperCache.Clear();
            }
        }

        #endregion

        #region MapCore

        internal static TTarget MapCore<TSource, TTarget>(TSource source, TTarget target, int depth)
        {
            if(source == null || depth > mapperConfig.MaxDepth)
            {
                return target;
            }
            TypePair pair = TypePair.Create<TSource, TTarget>();
            IMapper mapper = MapperUtil.GetMapper(pair);
            return (TTarget)mapper.Map(source, target, mapperConfig.ReferencePropertyHandle, depth + 1);
        }

        internal static IEnumerable<TTarget> MapCores<TSource, TTarget>(IEnumerable<TSource> source, int depth)
        {
            if(source == null || depth > mapperConfig.MaxDepth)
            {
                return null;
            }
            TypePair pair = TypePair.Create<TSource, TTarget>();
            IMapper mapper = MapperUtil.GetMapper(pair);
            var list = new List<TTarget>();
            foreach(TSource item in source)
            {
                TTarget rst = (TTarget)mapper.Map(item, default, mapperConfig.ReferencePropertyHandle, depth + 1);
                list.Add(rst);
            }
            return list;
        }

        internal static TTarget MoveCollection<TTarget, TItem>(IEnumerable<TItem> source, TTarget target = default) where TTarget : IEnumerable<TItem>
        {
            if(source == null)
            {
                return default;
            }
            var type = typeof(TTarget);
            if(type.IsArray)
            {
                return MoveArrayInner<TTarget, TItem>(source, target);
            }
            else
            {
                return MoveCollectionInner<TTarget, TItem>(source, target);
            }
        }
        private static TTarget MoveCollectionInner<TTarget, TItem>(IEnumerable<TItem> source, TTarget target = default) where TTarget : IEnumerable<TItem>
        {
            var type = typeof(TTarget);
            if(target == null)
            {
                var newFunc = CollectionActions<TTarget, TItem>.New;
                if(newFunc == null)
                {
                    throw new Exception($"不支持的目标类型 {type.FullName}");
                }
                target = newFunc(source.ToList());
            }
            else
            {
                var clearFunc = CollectionActions<TTarget, TItem>.Clear;
                var addItemFunc = CollectionActions<TTarget, TItem>.AddItem;
                if(clearFunc == null || addItemFunc == null)
                {
                    throw new Exception($"不支持的目标类型 {type.FullName}");
                }
                clearFunc(target);
                foreach(var item in source)
                {
                    addItemFunc(target, item);
                }
            }

            return target;
        }
        internal static TTarget MoveArrayInner<TTarget, TItem>(IEnumerable<TItem> source, TTarget target = default) where TTarget : IEnumerable<TItem>
        {
            return (TTarget)(object)source.ToArray();
        }
        internal static TValue GetExpressionResult<TSource, TTarget, TValue>(string name, TSource source)
        {
            return ExpressionCache<TSource, TTarget>.GetValue<TValue>(name, source);
        }

        #endregion

        #region Events

        private static void PreMap(object obj)
        {
            Logger?.Invoke($"源数据：\r\n{Object2String(obj)}");
        }

        private static void PostMap(object obj)
        {
            Logger?.Invoke($"目标数据：\r\n{Object2String(obj)}");
        }
        private static void OnError(Exception ex)
        {
            Logger?.Invoke($"映射发生错误：{ex.Message}\r\n堆栈信息：\r\n{ex.StackTrace}");
        }

        #endregion
    }
}
