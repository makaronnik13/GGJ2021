using System;
using System.Collections.Generic;
using System.Linq;

namespace com.armatur.common.util
{
    public static class EnumerableExtension
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).SingleOrDefault();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }


        public static string ArrayString<T>(this IEnumerable<T> collection)
        {
            string str = "";
            foreach (var item in collection.ToArray())
            {
                str += item.ToString() + ", ";
            }

            return str;
        }

        public static string ArrayString<T>(this IList<T> collection)
        {
            string str = "";
            foreach (var item in collection.ToArray())
            {
                str += item.ToString() + ", ";
            }
            str = str.Trim(' ');
            str = str.Trim(',');
            return str;
        }
    }


}