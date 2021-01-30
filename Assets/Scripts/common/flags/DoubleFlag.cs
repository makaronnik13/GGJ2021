using System;

namespace com.armatur.common.flags
{
    public class DoubleFlag : GenericFlag<double>
    {
        public DoubleFlag(string name = null, double value = 0) : base(name, value)
        {
            AddListener(d => ReverseIntFlag.State = -(int) Math.Ceiling(Value));
        }

        public GenericFlag<int> ReverseIntFlag { get; } = new GenericFlag<int>("intFlag", 0);
    }
}