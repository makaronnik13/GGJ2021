using System;
using com.armatur.common.logic;

namespace com.armatur.common.flags
{
    public class GenericFlagComparable<T> : ChangingValueComparable<T> where T : IComparable
    {
        public GenericFlagComparable(string name, T value) : base(name, value)
        {
        }

        public T State
        {
            get { return Value; }
            set { SetInnerState(value); }
        }

        public void SetState(T value)
        {
            State = value;
        }
    }

    [System.Serializable]
    public class GenericFlag<T> : ChangingValue<T>
    {
        public GenericFlag(string name, T value) : base(name, value)
        {
        }

        public T State
        {
            get { return Value; }
            set { SetInnerState(value); }
        }

        public void SetState(T value)
        {
            State = value;
        }

        public new void RaiseEvent()
        {
            base.RaiseEvent();
        }
    }



    public class SimpleFlag : GenericFlagComparable<bool>
    {
        public SimpleFlag(string name = null, bool state = false) : base(name, state)
        {
        }

        public void Reverse()
        {
            Value = !Value;
        }

        public static implicit operator bool(SimpleFlag flag)
        {
            return flag.Value;
        }
    }

    public class IntFlag : GenericFlag<int>
    {
        private readonly Lazy<FlagUpdater<bool>> _emptyFlag;
        private readonly Lazy<FlagUpdater<bool>> _nonEmptyFlag;

        public IntFlag(string name = null, int value = 0) : base(name, value)
        {
            _emptyFlag = new Lazy<FlagUpdater<bool>>(() => ChangingValueFlag<int>.CreateFixed(this, LogicOperation.Equal, 0, Name + " empty"));
            _nonEmptyFlag = new Lazy<FlagUpdater<bool>>(() => ChangingValueFlag<int>.CreateFixed(this, LogicOperation.NotEqual, 0, Name + " not empty"));
        }

        public FlagUpdater<bool> GetEmptyFlag()
        {
            return _emptyFlag.Value;
        }

        public FlagUpdater<bool> GetNotEmptyFlag()
        {
            return _nonEmptyFlag.Value;
        }
    }
}