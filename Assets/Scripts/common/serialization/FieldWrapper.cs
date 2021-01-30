using System.Reflection;

namespace com.armatur.common.serialization
{
    public class FieldWrapper : MemberWrapper
    {
        private readonly FieldInfo _fieldInfo;

        public FieldWrapper(FieldInfo fieldInfo) : base(fieldInfo, fieldInfo.FieldType)
        {
            _fieldInfo = fieldInfo;
        }

        public override void SetValue(object parentObject, object value)
        {
            _fieldInfo.SetValue(parentObject, value);
        }

        public override object GetValue(object parentObject)
        {
            return _fieldInfo.GetValue(parentObject);
        }
    }
}