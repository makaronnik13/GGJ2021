using System.Reflection;

namespace com.armatur.common.serialization
{
    public class StringConvertation : ISerializerStringConverter
    {
        private readonly MethodBase _fromString;
        private readonly MethodBase _toString;

        public StringConvertation(IReflect type, string staticMethodName, string stringName)
        {
            _fromString = type.GetMethod(staticMethodName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            _toString = type.GetMethod(stringName,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }

        public object ConvertFromString(string xml)
        {
            return _fromString.Invoke(null, new object[] {xml});
        }

        public string ConvertToString(object obj)
        {
            if (obj != null)
                return (string) _toString.Invoke(obj, null);
            return null;
        }
    }
}