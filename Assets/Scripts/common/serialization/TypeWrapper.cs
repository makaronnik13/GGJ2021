using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.armatur.common.save;
using com.armatur.common.util;

namespace com.armatur.common.serialization
{
    public abstract class TypeWrapper
    {
        private readonly IDictionary<string, Type> _knownTypes = new Dictionary<string, Type>();
        private readonly string _name;
        public readonly Type Type;
        public ISerializerStringConverter Converter;

        public IEnumerable<string> AvailableTypes => _availableTypes;

        private readonly List<string> _availableTypes;

        protected TypeWrapper(Type type)
        {
            Type = type;
            var attr = (StaticDictionaryAttribute) type.GetCustomAttributes(typeof(StaticDictionaryAttribute), true)
                .FirstOrDefault();
            if (attr != null)
                Converter = new StringConvertation(type, attr.InitializerName, attr.StringName);
            try
            {
                (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from assemblyType in domainAssembly.GetTypes()
                    where Type.IsAssignableFrom(assemblyType) && !assemblyType.GetTypeInfo().IsAbstract
                    select assemblyType).ForEach(t =>_knownTypes.Add(SerializeUtils.GetTypeFriendlyName(t), t));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            _name = SerializeUtils.GetTypeFriendlyName(Type);
            _availableTypes = _knownTypes.Keys.ToList();
        }

        public TypeWrapper GetPolymorphicTypeWrapper(string name)
        {
            TypeWrapper typeWrapper = null;
            Type type;
            if (name == _name)
                typeWrapper = this;
            else if (_knownTypes.TryGetValue(name, out type))
                typeWrapper = TypeWrapperPool.Instance.GetWrapper(type);
            return typeWrapper;
        }

        public static TypeWrapper Create(Type type)
        {
            if (type.IsBasicType())
                return new BasicTypeWrapper(type);
            if (type.IsCollectionType())
                return new CollectionTypeWrapper(type);
            return new ComplexTypeWrapper(type);
        }

        public string Name()
        {
            return _name;
        }

        protected abstract bool IsOneValueInternal();
        protected abstract string GetInternalStringValue(object o);
        protected abstract bool SetInternalStringValue(ref object obj, string value);

        public bool IsOneValue()
        {
            return Converter != null || IsOneValueInternal();
        }

        public string GetStringValue(object o)
        {
            return Converter != null ? Converter.ConvertToString(o) : GetInternalStringValue(o);
        }

        public bool SetStringValue(ref object o, string value)
        {
            bool res;
            if (Converter != null)
            {
                o = Converter.ConvertFromString(value);
                res = o != null;
            }
            else
                res = SetInternalStringValue(ref o, value);

            return res;
        }

        public virtual object CreateDefault()
        {
            return default(Type);
        }

        public abstract void Save(SaveProcessor processor, object o);
        public abstract bool Load(SaveProcessor processor, ref object o);
    }
}