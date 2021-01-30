using System;
using System.Collections.Generic;
using System.Linq;

namespace com.armatur.common.enums
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }


        public static int Index(this Enum value)
        {
            return Array.IndexOf(Enum.GetValues(value.GetType()), value);
        }
    }
}