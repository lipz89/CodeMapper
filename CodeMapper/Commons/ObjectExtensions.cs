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
    }
}
