using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.armatur.common.save;
using com.armatur.common.util;

namespace com.armatur.common.serialization
{
    public class ComplexTypeWrapper : TypeWrapper
    {
        public readonly List<MemberWrapper> Members = new List<MemberWrapper>();

        public ComplexTypeWrapper(Type type) : base(type)
        {
            foreach (
                var memberInfo in
                SharpUtil.GetMembers(Type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(member => member.GetCustomAttributes(typeof(SerializeDataAttribute), true).Length > 0)
                    )
            {
                var wrapper = MemberWrapper.Create(memberInfo);
                if (wrapper != null)
                    Members.Add(wrapper);
            }

            var testSet = new HashSet<string>();
            Members.ForEach(memberWrapper =>
            {
                if (!(memberWrapper.AllowInheritance && memberWrapper.OmitOuterName))
                    return;
                memberWrapper.GetAvailableTypes().ForEach(s =>
                {
                    if (!testSet.Add(s))
                        throw new Exception(SerializeUtils.GetTypeFriendlyName(Type) + ": Several members with duplicate known types " + s);
                });
            });
        }

        public override object CreateDefault()
        {
            try
            {
                return Type.InvokeMember(string.Empty,
                    BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, null, new object[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override bool IsOneValueInternal()
        {
            return Members.Count == 1 && Members[0].IsOneValue();
        }

        protected override string GetInternalStringValue(object o)
        {
            if (IsOneValueInternal())
                return Members[0].GetStringValue(o);
            throw new SerializeExceptions.NoStringRepresentation();
        }

        protected override bool SetInternalStringValue(ref object obj, string value)
        {
            if (IsOneValueInternal())
                return Members[0].SetStringValue(obj, value);
            throw new SerializeExceptions.NoStringRepresentation();
        }

        public override void Save(SaveProcessor processor, object o)
        {
            Members.ForEach(m => m.ProcessSave(processor, o));

            foreach (var source in SharpUtil
                .GetMethods(Type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(t => t.GetCustomAttributes(typeof(OnSerialize), true).Length > 0)
                )

                source.Invoke(o, new object[] { processor });
        }

        public override bool Load(SaveProcessor processor, ref object o)
        {
            foreach (var member in Members)
            {
                member.ProcessLoad(processor, o);
            }

            foreach (var source in SharpUtil
                .GetMethods(Type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(t => t.GetCustomAttributes(typeof(OnDeserialize), true).Length > 0)
                )

                source.Invoke(o, new object[] {processor});

            return true;
        }
    }
}