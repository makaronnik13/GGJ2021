using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using com.armatur.common.save;
using com.armatur.common.util;
using com.armatur.common.serialization;

namespace com.armatur.common.enums
{
    [StaticDictionary("get_Name", "GetByName")]
    public class EnumedExample<T> : Enumed where T : EnumedExample<T>
    {
        private static readonly FlaggedList<T> ListValues = new FlaggedList<T>();

        // ReSharper disable once StaticMemberInGenericType
        private static int _counter;

        static EnumedExample()
        {
            RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        }

        protected EnumedExample(string name)
        {
            Name = name;
        }

        public static ReadOnlyCollection<T> Values => ListValues.ReadOnly;

        public static T GetByName(string name)
        {
            return ListValues.FirstOrDefault(enumExample => enumExample.Name.Equals(name));
        }

        protected static void AddEnum(T enumExample)
        {
            ListValues.Add(enumExample);
            enumExample.Index = _counter++;
        }

        public static T CreateFromConfig(SaveProcessor processor)
        {
            return GetByName(processor.GetAnyValue(typeof(T).Name));
        }
    }
}