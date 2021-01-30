using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.armatur.common.util
{
    public static class ReflectionUtils
    {
        public static bool IsSimple(this Type type)
        {
            while (true)
            {
                var typeInfo = type.GetTypeInfo();
                if (!typeInfo.IsGenericType || typeInfo.GetGenericTypeDefinition() != typeof(Nullable<>))
                    return typeInfo.IsPrimitive || typeInfo.IsEnum || type == typeof(string) || type == typeof(decimal);
                // nullable type, check if the nested type is simple.
                type = typeInfo.GetGenericArguments()[0];
            }
        }

        public static bool IsList(object o)
        {
            if (o == null) return false;
            var interfaceTest =
                new Predicate<Type>(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
            var type = o.GetType();
            return interfaceTest(type) || type.GetInterfaces().Any(i => interfaceTest(i));
        }

        public static bool IsCollectionType(this Type type)
        {
            return type != typeof(string) && type.IsIEnumerable();
        }

        public static bool IsIEnumerable(this Type type)
        {
            Type seqType;
            return type.IsIEnumerable(out seqType);
        }

        public static bool IsArray(this Type type, out Type elementType)
        {
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            if (type == typeof(Array)) // i.e., a direct ref to System.Array
            {
                elementType = typeof(object);
                return true;
            }

            elementType = typeof(object);
            return false;
        }

        public static bool IsIEnumerable(this Type type, out Type seqType)
        {
            // detect arrays early
            if (type.IsArray(out seqType))
                return true;

            seqType = typeof(object);
            if (type == typeof(IEnumerable))
                return true;

            var isNongenericEnumerable = false;

            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                seqType = type.GetGenericArguments()[0];
                return true;
            }

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var genArgs = interfaceType.GetGenericArguments();
                    seqType = genArgs[0];
                    return true;
                }
                else if (interfaceType == typeof(IEnumerable))
                {
                    isNongenericEnumerable = true;
                }

            // the second case is a direct reference to IEnumerable
            if (!isNongenericEnumerable && type != typeof(IEnumerable)) return false;

            seqType = typeof(object);
            return true;
        }

        public static object InvokeMethod(object srcObj, string methodName, params object[] args)
        {
            var argTypes = args.Select(x => x.GetType()).ToArray();
            var method = srcObj.GetType().GetMethod(methodName, argTypes);
            var result = method.Invoke(srcObj, args);
            return result;
        }
    }
}