using System;
using System.Collections.Generic;

namespace CodeMapper.Commons
{
    /// <summary>
    /// 全局缓存
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Cache<TKey, TValue> where TValue : class
    {
        private IDictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public bool HasKey(TKey key)
        {
            lock(dictionary)
            {
                return dictionary.ContainsKey(key);
            }
        }
        public TValue Get(TKey key)
        {
            lock(dictionary)
            {
                TValue result;
                bool exsist = dictionary.TryGetValue(key, out result);
                if(exsist)
                {
                    return result;
                }
                return null;
            }
        }
        public void Remove(TKey key)
        {
            if(dictionary.ContainsKey(key))
            {
                lock(dictionary)
                {
                    if(dictionary.ContainsKey(key))
                    {
                        dictionary.Remove(key);
                    }
                }
            }
        }
        public TValue GetOrAdd(TKey key, Func<TValue> creator)
        {
            lock(dictionary)
            {
                TValue result;
                bool exsist = dictionary.TryGetValue(key, out result);
                if(exsist)
                {
                    return result;
                }
                if(creator == null)
                {
                    throw new ArgumentNullException(nameof(creator));
                }
                result = creator();
                if(!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, result);
                }
                else
                {
                    dictionary[key] = result;
                }
                return result;
            }
        }
        public void Add(TKey key, TValue value)
        {
            if(!dictionary.ContainsKey(key))
            {
                lock(dictionary)
                {
                    if(!dictionary.ContainsKey(key))
                    {
                        dictionary[key] = value;
                    }
                }
            }
        }

        public void Clear()
        {
            lock(dictionary)
            {
                dictionary.Clear();
            }
        }
    }
}
