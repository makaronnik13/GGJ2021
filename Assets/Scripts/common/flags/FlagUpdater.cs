using System;
using com.armatur.common.save;
using com.armatur.common.serialization;

namespace com.armatur.common.flags
{
    public abstract class FlagUpdater<T> : ChangingValue<T> where T : IComparable
    {
        public FlagUpdater(string name, T value) : base(name, value)
        {
        }

        protected void Update<TV>(TV value)
        {
            Update();
        }

        protected void Update()
        {
            SetInnerState(NewValue());
        }

        [OnDeserialize]
        public void OnDeserialize(SaveProcessor processor)
        {
            Update();
        }

        protected abstract T NewValue();
    }
}