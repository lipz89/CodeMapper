﻿using CodeMapper.Builders;
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
        private static IMapperConfig mapperConfig = new MapperConfig();
        private static MapperBuilder targetMapperBuilder = MapperBuilder.GetMapperBuilder(mapperConfig);

        internal static IMapper GetMapper(TypePair pair)
        {
            return Cache<TypePair, IMapper>.GetOrAdd(pair, () => GetMapperCore(pair));
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

        //private static IMapper GetClassMapper(TypePair pair)
        //{
        //    var className = pair.GetClassName();
        //    return IocMappers.Instance.ClassMappers.FirstOrDefault(x => x.GetType().Name == className);
        //}
        //internal static IMapper<TSource, TTarget> GetMapper<TSource, TTarget>()
        //{
        //    TypePair pair = TypePair.Create<TSource, TTarget>();
        //    IMapper mapper;
        //    var has = _mappers.TryGetValue(pair, out mapper);
        //    if(!has)
        //    {
        //        var imapper = GetMapperCore<TSource, TTarget>();
        //        _mappers.Add(pair, imapper);
        //        return imapper;
        //    }
        //    return (IMapper<TSource, TTarget>)mapper;
        //}

        //private static IMapper<TSource, TTarget> GetMapperCore<TSource, TTarget>()
        //{
        //    TypePair pair = TypePair.Create<TSource, TTarget>();
        //    if(Converter.IsConvertibleType(pair))
        //    {
        //        return new ConvertibleMapper<TSource, TTarget>();
        //    }
        //    if(pair.IsEnumerableTypes)
        //    {
        //        return new CollectionMapper<TSource, TTarget>();
        //    }
        //    return GetClassMapper<TSource, TTarget>();
        //}

        //private static IMapper<TSource, TTarget> GetClassMapper<TSource, TTarget>()
        //{
        //    TypePair pair = TypePair.Create<TSource, TTarget>();
        //    var className = pair.GetClassName();
        //    var mapper = IocMappers.Instance.ClassMappers.FirstOrDefault(x => x.GetType().Name == className);
        //    return mapper as IMapper<TSource, TTarget>;
        //}

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
        }

        #endregion

        #region MapCore

        internal static TTarget MapCore<TSource, TTarget>(TSource source, TTarget target = default)
        {
            if(source == null)
            {
                return target;
            }
            TypePair pair = TypePair.Create<TSource, TTarget>();
            IMapper mapper = MapperUtil.GetMapper(pair);
            return (TTarget)mapper.Map(source, target);
        }

        internal static IEnumerable<TTarget> MapCores<TSource, TTarget>(IEnumerable<TSource> source)
        {
            if(source == null)
            {
                return null;
            }
            TypePair pair = TypePair.Create<TSource, TTarget>();
            IMapper mapper = MapperUtil.GetMapper(pair);
            var list = new List<TTarget>();
            foreach(TSource item in source)
            {
                TTarget rst = (TTarget)mapper.Map(item, default);
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
    }
}
