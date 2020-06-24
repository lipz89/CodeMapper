using System;
using System.Collections;

namespace CodeMapper.Commons
{
    internal static class ObjectExtensions
    {
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }
        public static int Count(this IEnumerable source)
        {
            var collection = source as ICollection;
            if(collection != null)
            {
                return collection.Count;
            }

            var count = 0;
            foreach(object item in source)
            {
                count++;
            }
            return count;
        }

        public static T Between<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if(min.CompareTo(max) > 0)
            {
                var tmp = max;
                max = min;
                min = tmp;
            }
            if(min.CompareTo(value) > 0)
            {
                return min;
            }
            if(value.CompareTo(max) > 0)
            {
                return max;
            }
            return value;
        }
        public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(value.Between(min, max)) == 0;
        }
    }
}
