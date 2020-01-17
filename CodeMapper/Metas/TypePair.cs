using CodeMapper.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CodeMapper.Metas
{
    internal struct TypePair : IEquatable<TypePair>
    {
        private static List<Type> convertableTypes = new List<Type>
        {
            typeof(string),
            typeof(decimal) ,
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        };
        public TypePair(Type source, Type target) : this()
        {
            if(!source.IsPublic())
            {
                throw new Exception(string.Format("类型 {0} 未公开，不能映射。", source.FullName));
            }

            if(!target.IsPublic())
            {
                throw new Exception(string.Format("类型 {0} 未公开，不能映射。", target.FullName));
            }
            Target = target;
            Source = source;
        }

        public bool IsBaseTypes
        {
            get
            {
                if(IsValueTypes && IsPrimitiveTypes)
                {
                    return true;
                }
                if(IsEnumTypes)
                {
                    return true;
                }
                else if(convertableTypes.Contains(Source) || convertableTypes.Contains(Target))
                {
                    return true;
                }
                else if(IsNullableTypes)
                {
                    var nullablePair = new TypePair(Nullable.GetUnderlyingType(Source), Nullable.GetUnderlyingType(Target));
                    return nullablePair.IsBaseTypes;
                }
                return false;
            }
        }

        public bool IsEnumTypes
        {
            get { return Source.IsEnum && Target.IsEnum; }
        }

        public bool IsEnumerableTypes
        {
            get { return Source.IsIEnumerable() && Target.IsIEnumerable(); }
        }

        public bool IsNullableToNotNullable
        {
            get { return Source.IsNullable() && Target.IsNullable() == false; }
        }

        public Type Source { get; private set; }
        public Type Target { get; private set; }

        internal bool IsEqualTypes
        {
            get { return Source == Target; }
        }

        internal bool IsNullableTypes
        {
            get { return Source.IsNullable() && Target.IsNullable(); }
        }

        internal bool IsPrimitiveTypes
        {
            get { return Source.IsPrimitive && Target.IsPrimitive; }
        }

        internal bool IsValueTypes
        {
            get { return Source.IsValueType && Target.IsValueType; }
        }

        public static TypePair Create(Type source, Type target)
        {
            return new TypePair(source, target);
        }

        public static TypePair Create<TSource, TTarget>()
        {
            return new TypePair(typeof(TSource), typeof(TTarget));
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is TypePair && Equals((TypePair)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Source != null ? Source.GetHashCode() : 0) * 397) ^ (Target != null ? Target.GetHashCode() : 0);
            }
        }

        public bool HasTypeConverter()
        {
            TypeConverter fromConverter = TypeDescriptor.GetConverter(Source);
            if(fromConverter.CanConvertTo(Target))
            {
                return true;
            }

            TypeConverter toConverter = TypeDescriptor.GetConverter(Target);
            if(toConverter.CanConvertFrom(Source))
            {
                return true;
            }
            return false;
        }

        public bool Equals(TypePair other)
        {
            return Source == other.Source && Target == other.Target;
        }

        public override string ToString()
        {
            return GetTypeName(Source) + "_" + GetTypeName(Target);
        }

        public string GetClassName()
        {
            return "Mapper_" + this.ToString();
        }

        private string GetTypeName(Type type)
        {
            if(type == null)
            {
                return string.Empty;
            }

            if(type == typeof(void))
            {
                return "void";
            }

            if(type.IsGenericParameter)
            {
                return type.Name;
            }

            if(!type.IsGenericType)
            {
                return type.Name;
            }

            var str = type.Name;
            var gp = type.GetGenericArguments();
            foreach(var type1 in gp)
            {
                str += "_" + GetTypeName(type1);
            }

            return str;
        }
    }
}
