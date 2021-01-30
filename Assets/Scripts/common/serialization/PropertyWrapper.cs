using System.Reflection;

namespace com.armatur.common.serialization
{
    public class PropertyWrapper : MemberWrapper
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyWrapper(PropertyInfo propertyInfo) : base(propertyInfo, propertyInfo.PropertyType)
        {
            _propertyInfo = propertyInfo;
        }

        public override void SetValue(object parentObject, object value)
        {
            _propertyInfo.SetValue(parentObject, value, null);
        }

        public override object GetValue(object parentObject)
        {
            return _propertyInfo.GetValue(parentObject, null);
        }
    }
}