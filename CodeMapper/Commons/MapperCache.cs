using System;
using System.Collections.Generic;
using System.Threading;

namespace CodeMapper.Commons
{
    internal static class MapperCache
    {
        private interface IInner
        {
            void Clear();
        }

        private class Inner<T> : IInner
        {
            private static Inner<T> instance;

            public static Inner<T> Instance
            {
                get
                {
                    if(instance == null)
                    {
                        instance = new Inner<T>();
                        Inners.Add(instance);
                    }
                    return instance;
                }
            }

            private readonly ThreadLocal<IDictionary<long, T>> localDictionary = new ThreadLocal<IDictionary<long, T>>(() => new Dictionary<long, T>());

            internal IDictionary<long, T> Dictionary
            {
                get { return localDictionary.Value; }
            }

            void IInner.Clear()
            {
                localDictionary?.Value?.Clear();
            }
        }

        private static List<IInner> Inners { get; } = new List<IInner>();

        internal static void Clear()
        {
            foreach(var inner in Inners)
            {
                inner?.Clear();
            }
        }

        internal static bool TryGet<T>(long key, out T t)
        {
            return Inner<T>.Instance.Dictionary.TryGetValue(key, out t);
        }

        internal static void Set<T>(long key, T t)
        {
            if(!Inner<T>.Instance.Dictionary.ContainsKey(key))
            {
                Inner<T>.Instance.Dictionary.Add(key, t);
            }
        }

        internal static T Get<T>(long key, Func<T> creator)
        {
            if(!Inner<T>.Instance.Dictionary.ContainsKey(key))
            {
                var t = creator();
                Inner<T>.Instance.Dictionary.Add(key, t);
            }
            return Inner<T>.Instance.Dictionary[key];
        }
    }
}
