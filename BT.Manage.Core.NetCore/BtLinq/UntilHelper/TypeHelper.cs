using System;
using System.Linq;

namespace BT.Manage.Core
{
    public class TypeHelper
    {
        public static Type GetUnderlyingType(Type type)
        {
            if (!type.IsGenericType)
            {
                return type;
            }
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static bool IsCompilerGenerated(Type type)
        {
            return type.GetCustomAttributes(ReflectorConsts.CompilerGeneratedAttributeType, false).Any();
        }

        public static bool IsEnum(Type propertyType)
        {
            propertyType = GetUnderlyingType(propertyType);
            return propertyType.IsEnum;
        }

        public static bool IsNullableType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsGenericType)
            {
                return false;
            }
            return type.GetGenericTypeDefinition() == ReflectorConsts.NullableType;
        }

        public static bool IsValueType(Type type)
        {
            type = GetUnderlyingType(type);
            if (!type.IsPrimitive && !(type == ReflectorConsts.DateTimeType) && !(type == ReflectorConsts.StringType))
            {
                return false;
            }
            return true;
        }
    }
}