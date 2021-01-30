using System;

namespace com.armatur.common.serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StaticDictionaryAttribute : Attribute
    {
        public string InitializerName;
        public string StringName;

        public StaticDictionaryAttribute(string stringName, string initializerName)
        {
            InitializerName = initializerName;
            StringName = stringName;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnDeserialize : SerializeAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnSerialize : SerializeAttribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public abstract class SerializeAttribute : Attribute
    {
    }

    public enum FieldRequired
    {
        True,
        False
    }

    public enum FieldOmitName
    {
        True,
        False
    }

    public enum ComplexType
    {
        True,
        False
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SavableAttribute : Attribute
    {
        public ComplexType isComplex = ComplexType.False;

        public SavableAttribute(ComplexType isComplex = ComplexType.False)
        {
            this.isComplex = isComplex;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeDataAttribute : SerializeAttribute
    {
        public readonly bool AsAttribute;
        public readonly string Name;
        public readonly bool OmitOuterName;
        public readonly bool Required;
        public readonly bool Inherited;

        public SerializeDataAttribute(FieldOmitName omitOuterName, FieldRequired required = FieldRequired.True)
            : this(null, false, required, omitOuterName)
        {
        }

        public SerializeDataAttribute(string name, FieldRequired required = FieldRequired.True)
            : this(name, false, required)
        {
        }

        protected SerializeDataAttribute(string name, bool asAttribute = false, FieldRequired required = FieldRequired.True,
            FieldOmitName omitOuterName = FieldOmitName.False, bool inherited = false)
        {
            Name = name;
            AsAttribute = asAttribute;
            Required = required == FieldRequired.True;
            OmitOuterName = omitOuterName == FieldOmitName.True;
            Inherited = inherited;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeInheritedAttribute : SerializeDataAttribute
    {
        public SerializeInheritedAttribute(string name, FieldRequired required = FieldRequired.True)
            : base(name, false, required, FieldOmitName.False, true)
        {
        }

        public SerializeInheritedAttribute(FieldOmitName omitOuterName, FieldRequired required = FieldRequired.True)
            : base(null, false, required, omitOuterName, true)
        {
        }

        public SerializeInheritedAttribute(FieldRequired required)
            : base(null, false, required, FieldOmitName.True, true)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeAsAttributeAttribute : SerializeDataAttribute
    {
        public SerializeAsAttributeAttribute(FieldOmitName omitOuterName, FieldRequired required = FieldRequired.True)
            : base(null, true, required, omitOuterName)
        {
        }

        public SerializeAsAttributeAttribute(string name, FieldRequired required = FieldRequired.True)
            : base(name, true, required)
        {
        }

        public SerializeAsAttributeAttribute(FieldRequired required)
            : base(null, true, required)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeCollectionAttribute : SerializeDataAttribute
    {
        public SerializeCollectionAttribute(FieldOmitName omitOuterName, FieldRequired required = FieldRequired.True) : base(null, false, required, omitOuterName)
        {
        }

        public SerializeCollectionAttribute(FieldRequired required) : base(null, false, required)
        {
        }

        public SerializeCollectionAttribute(string name, FieldRequired required = FieldRequired.True) : base(name, false, required)
        {
        }
    }
}