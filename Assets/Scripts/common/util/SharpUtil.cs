using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace com.armatur.common.util
{
    public static class SharpUtil
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
                action(element);
        }

        public static void Each<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            foreach (var element in source)
                action(element, i++);
        }
        
        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess)
        {
            return ((MemberExpression) memberAccess.Body).Member.Name;
        }


        public static IEnumerable<MethodInfo> GetMethods(Type t, BindingFlags bindingAttr)
        {
            if (t == null)
                return Enumerable.Empty<MethodInfo>();

            return t.GetMethods(bindingAttr).Concat(GetMethods(t.BaseType, bindingAttr));
        }

        public static IEnumerable<MemberInfo> GetMembers(Type t, BindingFlags bindingAttr)
        {
            if (t == null)
                return Enumerable.Empty<MemberInfo>();

            return t.GetMembers(bindingAttr).Where(info => info.DeclaringType == t)
                .Concat(GetMembers(t.BaseType, bindingAttr)).ToList();
        }

        public static string GetName<T>(this T item) where T : class
        {
            return typeof(T).GetProperties()[0].Name;
        }
    }
}