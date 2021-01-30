using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.armatur.common.save;
using com.armatur.common.util;

namespace com.armatur.common.serialization
{
    public abstract class MemberWrapper
    {
        private readonly MemberInfo membInfo;
        private readonly bool _isAttribute;
        public readonly string Name;
        public readonly bool AllowInheritance;
        public readonly bool OmitOuterName;
        public readonly TypeWrapper TypeWrapper;
        private readonly bool _required;
        public readonly Type MemberType;

        public static Queue<string> memberLoadingMem = new Queue<string>(10);

        protected MemberWrapper(MemberInfo memberInfo, Type type)
        {
            this.membInfo = memberInfo;
            Name = memberInfo.Name;
            MemberType = type;
            TypeWrapper = TypeWrapperPool.Instance.GetWrapper(MemberType);
            AllowInheritance = TypeWrapper.AvailableTypes.Count() > 1;
            var first =
                (SerializeDataAttribute) memberInfo.GetCustomAttributes(typeof(SerializeDataAttribute), true).First();
            if (null != first)
            {
                _required = first.Required;
                _isAttribute = first.AsAttribute;
                OmitOuterName = first.OmitOuterName;
                AllowInheritance = AllowInheritance && !_isAttribute && first.Inherited;
                if (!string.IsNullOrEmpty(first.Name))
                    Name = first.Name;
            }
        }

        public bool IsOneValueView()
        {
            return _isAttribute && TypeWrapper.IsOneValue();
        }

        public IEnumerable<string> GetAvailableTypes()
        {
            return TypeWrapper.AvailableTypes;
        }

        public void ProcessSave(SaveProcessor processor, object parentObject)
        {
            var o = GetValue(parentObject);

            if (processor.IsSavableOnly)
            {
                var savableAttr = (SavableAttribute)membInfo.GetCustomAttributes(typeof(SavableAttribute), true).FirstOrDefault();
                if (savableAttr == null)
                    return;
            }

            if (o == null) return;

            //UberDebug.Log("saving member: " + membInfo.Name);

            var typeWrapper = TypeWrapperPool.Instance.GetWrapper(o.GetType());
            if (_isAttribute && typeWrapper.IsOneValue())
            {
                processor.AddField(Name, typeWrapper.GetStringValue(o));
            }
            else
            {
                processor.AddLevel(Name, OmitOuterName, true);
                if (AllowInheritance)
                    processor.AddLevel(typeWrapper.Name(), false, true);
                typeWrapper.Save(processor, o);
                if (AllowInheritance)
                    processor.RemoveOneLevel(false);
                processor.RemoveOneLevel(OmitOuterName);
            }
        }

        public void ProcessLoad(SaveProcessor processor, object parentObject)
        {
            //UberDebug.Log("loading member: " + membInfo.Name);

            memberLoadingMem.Enqueue(this.membInfo.Name);
            if (memberLoadingMem.Count > 10)
                memberLoadingMem.Dequeue();

            var loaded = false;

            if (processor.IsSavableOnly)
            {
                var savableAttr = (SavableAttribute)membInfo.GetCustomAttributes(typeof(SavableAttribute), true).FirstOrDefault();
                if (savableAttr == null)
                    return;
            }

            if (processor.SkipSavables)
            {
                var savableAttr = (SavableAttribute)membInfo.GetCustomAttributes(typeof(SavableAttribute), true).FirstOrDefault();
                if (savableAttr != null)
                    return;
            }

            var o = GetValue(parentObject);

            if (_isAttribute)
            {
                if (!IsOneValue())
                    throw new Exception(SerializeUtils.GetTypeFriendlyName(TypeWrapper.GetType()) + ": Can't load as attribute : " + memberLoadingMem.ArrayString());
                if (null == o)
                    o = TypeWrapper.CreateDefault();
                var value = processor.GetField(Name);
                if (value != null)
                    loaded = TypeWrapper.SetStringValue(ref o, value);
            }
            else
            {
                if (AllowInheritance)
                {
                    var level = processor.AddLevel(Name, OmitOuterName, false);
                    if (level)
                    {
                        processor.ForEachLevel(s =>
                        {
                            var typeWrapper = TypeWrapper.GetPolymorphicTypeWrapper(s);
                            if (typeWrapper == null) return;
                            o = typeWrapper.CreateDefault();
                            loaded = typeWrapper.Load(processor, ref o);
                        });
                        processor.RemoveOneLevel(OmitOuterName);
                    }
                }
                else
                {
                    if (null == o)
                        o = TypeWrapper.CreateDefault();
                    var level = processor.AddLevel(Name, OmitOuterName, false);
                    if (level)
                    {
                        loaded = TypeWrapper.Load(processor, ref o);
                        processor.RemoveOneLevel(OmitOuterName);
                    }
                }
            }

            if (!loaded && _required)
                throw new Exception("Field " + Name + " in class " + SerializeUtils.GetTypeFriendlyName(parentObject.GetType()) + " is required, but wasn't loaded" + memberLoadingMem.ArrayString());

            SetValue(parentObject, o);
        }

        public bool IsOneValue()
        {
            return TypeWrapper.IsOneValue();
        }

        public string GetStringValue(object parentObject)
        {
            return TypeWrapper.GetStringValue(GetValue(parentObject));
        }

        public bool SetStringValue(object parentObject, string value)
        {
            var result = GetValue(parentObject) ?? TypeWrapper.CreateDefault();
            var res = TypeWrapper.SetStringValue(ref result, value);
            SetValue(parentObject, result);
            return res;
        }

        public abstract void SetValue(object parentObject, object value);

        public abstract object GetValue(object parentObject);

        public static MemberWrapper Create(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Field)
                return new FieldWrapper((FieldInfo) memberInfo);
            if (memberInfo.MemberType == MemberTypes.Property)
                return new PropertyWrapper((PropertyInfo) memberInfo);
            return null;
        }
    }
}