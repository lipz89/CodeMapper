using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeMapper.Commons
{
    internal static class MemberInfoExtensions
    {
        public static Type GetMemberType(this MemberInfo value)
        {
            if(value.IsField())
            {
                return ((FieldInfo)value).FieldType;
            }
            else if(value.IsProperty())
            {
                return ((PropertyInfo)value).PropertyType;
            }
            throw new NotSupportedException();
        }

        public static bool IsField(this MemberInfo value)
        {
            return value.MemberType == MemberTypes.Field;
        }

        public static bool IsProperty(this MemberInfo value)
        {
            return value.MemberType == MemberTypes.Property;
        }

        public static bool IsWritable(this MemberInfo value)
        {
            return value.IsField() || (((PropertyInfo)value).GetSetMethod() != null);
        }

        public static MemberInfo GetMemberInfo<T, TField>(this Expression<Func<T, TField>> expression)
        {
            var member = expression.Body as MemberExpression;
            if(member == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if(unaryExpression != null)
                {
                    member = unaryExpression.Operand as MemberExpression;
                }
            }
            return member?.Member;
        }
    }
}
