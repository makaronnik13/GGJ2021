using System;
using System.Collections.Generic;

namespace com.armatur.common.util
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetAllItems<T>(this Enum value)
        {
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }

    }
}