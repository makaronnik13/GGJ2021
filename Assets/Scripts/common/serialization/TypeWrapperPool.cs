using System;
using System.Collections.Generic;

namespace com.armatur.common.serialization
{
    public class TypeWrapperPool
    {
        public static readonly TypeWrapperPool Instance = new TypeWrapperPool();

        private readonly IDictionary<Type, TypeWrapper> _cache = new Dictionary<Type, TypeWrapper>();
        private readonly object _cacheLock = new object();

        public TypeWrapper GetWrapper(Type type)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            TypeWrapper result;
            if (_cache.TryGetValue(type, out result)) return result;
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(type, out result)) return result;
                result = TypeWrapper.Create(type);
                _cache.Add(type, result);
            }

            return result;
        }
    }
}