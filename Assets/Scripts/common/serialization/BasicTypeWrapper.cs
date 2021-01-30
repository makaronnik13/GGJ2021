using System;
using System.Globalization;
using com.armatur.common.save;

namespace com.armatur.common.serialization
{
    public class BasicTypeWrapper : TypeWrapper
    {
        public BasicTypeWrapper(Type type) : base(type)
        {
        }

        protected override bool IsOneValueInternal()
        {
            return true;
        }

        protected override string GetInternalStringValue(object o)
        {
            return Convert.ToString(o, CultureInfo.InvariantCulture);
        }

        protected override bool SetInternalStringValue(ref object obj, string value)
        {
            try
            {
                if (value != null)
                {
                    obj = Type.IsEnum
                        ? Enum.Parse(Type, value)
                        : Convert.ChangeType(value, Type, CultureInfo.InvariantCulture);

                    if (obj.GetType() == typeof(string))
                    {
                        obj = filterString(obj);
                    }
                }
                else
                    obj = null;

                return obj != null;
            }
            catch (Exception e)
            {
               // UberDebug.LogChannel("Exception", e.ToString());
            }

            return false;
        }

        public override void Save(SaveProcessor processor, object o)
        {
            processor.AddData(GetInternalStringValue(o));
        }

        public override bool Load(SaveProcessor processor, ref object o)
        {
            SetInternalStringValue(ref o, processor.GetData());
            return true;
        }

        private object filterString(object obj)
        {
            string str = (string)obj;
            str = str.Replace("\t", "");
            str = str.Replace("\n", "");

            return (object)str;
        }
    }
}