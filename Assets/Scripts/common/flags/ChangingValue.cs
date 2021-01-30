using System;
using com.armatur.common.events;
using com.armatur.common.logic;
using com.armatur.common.serialization;

namespace com.armatur.common.flags
{
    public class ChangingValueComparable<T> : ChangingValue<T>, IFlagComparable<T> where T : IComparable
    {
        public IFlag<bool> GetFlag(LogicOperation operation, IFlag<T> compareCounter, string name = null)
        {
            return new ChangingValueFlag<T>(this, operation, compareCounter, name);
        }

        public IFlag<bool> GetFixedFlag(LogicOperation operation, T compareValue, string name = null)
        {
            return ChangingValueFlag<T>.CreateFixed(this, operation, compareValue, name);
        }

        public ChangingValueComparable(string name, T value) : base(name, value)
        {
        }
    }

    public class ChangingValue<T> : IFlag<T>
    {
        protected readonly PriorityEvent<T> _event;
        private T _state;

        protected ChangingValue(string name, T value)
        {
            Name = name;
            _event = new PriorityEvent<T>(name + " changed");
            _state = value;
        }

        public string Name { get; }

        [Savable]
        [SerializeData(FieldOmitName.True, FieldRequired.False)]
        public T Value
        {
            get { return _state; }
            protected set { SetInnerState(value); }
        }

        public IListenerController AddListener(Action<T> listener, bool run = true, int priority = 0)
        {
            if (run)
                listener(Value);
            return _event.AddListener(listener, priority);
        }

        public void RemoveListener(Action<T> listener)
        {
            _event.RemoveListener(listener);
        }

        public IListenerController AddListener(Action listener, bool run = true)
        {
            if (run)
                listener();
            return _event.AddListener(listener);
        }

        public void RemoveListener(Action listener)
        {
            _event.RemoveListener(listener);
        }

        protected void SetInnerState(T value, bool fire = true)
        {
            // Set _state only if the new value different from the current (check for case value == _state == null as well).
            if (value?.Equals(_state) ?? _state == null) return;
            _state = value;
            if (fire)
                _event.Fire(_state);
        }

        public void RaiseEvent()
        {
            _event.Fire(_state);
        }
    }
}