using CodeMapper.Metas;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CodeMapper.Commons
{
    internal static class Converter
    {
        private static readonly Dictionary<TypePair, Func<object, object>> _dic = new Dictionary<TypePair, Func<object, object>>();

        public static bool IsConvertibleType(TypePair typePair)
        {
            return IsBaseType(typePair.Source) || typePair.HasTypeConverter();
        }

        private static bool IsBaseType(Type value)
        {
            return value.IsPrimitive
                   || value == typeof(string)
                   || value == typeof(Guid)
                   || value.IsEnum
                   || value == typeof(decimal)
                   || value.IsNullable() && IsBaseType(Nullable.GetUnderlyingType(value));
        }

        private static Func<object, object> ConvertEnum(TypePair pair)
        {
            Func<object, object> result;
            if(pair.Target.IsEnum)
            {
                if(pair.Source.IsEnum == false)
                {
                    if(pair.Source == typeof(string))
                    {
                        result = x => Enum.Parse(pair.Target, x.ToString());
                        return result;
                    }
                }
                result = x => Enum.ToObject(pair.Target, Convert.ChangeType(x, Enum.GetUnderlyingType(pair.Target)));
                return result;
            }
            if(pair.Source.IsEnum)
            {
                result = x => Convert.ChangeType(x, pair.Target);
                return result;
            }

            return null;
        }

        private static Func<object, object> GetConverter(TypePair pair)
        {
            TypeConverter fromConverter = TypeDescriptor.GetConverter(pair.Source);
            if(fromConverter.CanConvertTo(pair.Target))
            {
                return x => fromConverter.ConvertTo(x, pair.Target);
            }

            TypeConverter toConverter = TypeDescriptor.GetConverter(pair.Target);
            if(toConverter.CanConvertFrom(pair.Source))
            {
                return x => toConverter.ConvertFrom(x);
            }

            Func<object, object> enumConverter = ConvertEnum(pair);
            if(enumConverter != null)
            {
                return enumConverter;
            }

            if(pair.IsNullableToNotNullable)
            {
                return GetConverter(new TypePair(Nullable.GetUnderlyingType(pair.Source), pair.Target));
            }

            if(pair.Target.IsNullable())
            {
                return GetConverter(new TypePair(pair.Source, Nullable.GetUnderlyingType(pair.Target)));
            }

            return null;
        }

        public static Func<object, object> Get(TypePair pair)
        {
            return Cache<TypePair, Func<object, object>>.GetOrAdd(pair, () => GetConverter(pair));
        }
    }
}
