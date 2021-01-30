using System;
using System.Globalization;

namespace com.armatur.common.serialization
{
    public static class SerializeUtils
    {
        public static bool IsBasicType(this Type type)
        {
            return type == typeof(string) || type.IsPrimitive || type.IsEnum || type == typeof(DateTime) ||
                   type == typeof(decimal) ||
                   type == typeof(Guid);
        }

        public static string GetTypeFriendlyName(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var name = type.Name;
            if (type.IsGenericType)
            {
                var backqIndex = name.IndexOf('`');
                if (backqIndex == 0)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Bad type name: {0}",
                        name));
                if (backqIndex > 0) name = name.Substring(0, backqIndex);

                name += "Of";

                Array.ForEach(
                    type.GetGenericArguments(),
                    genType => name += GetTypeFriendlyName(genType));
            }
            else if (type.IsArray)
            {
                var t = type.GetElementType();
                name = string.Format(CultureInfo.InvariantCulture, "Array{0}Of{1}", type.GetArrayRank(),
                    GetTypeFriendlyName(t));
            }

            return name;
        }
    }
}