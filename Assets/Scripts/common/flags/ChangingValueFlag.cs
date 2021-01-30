using System;
using com.armatur.common.logic;

namespace com.armatur.common.flags
{
    public class ChangingValueFlag<T> : FlagUpdater<bool> where T : IComparable
    {
        private readonly IFlag<T> _compareCounter;
        private readonly IFlag<T> _counter;
        private readonly LogicOperation _operation;

        public ChangingValueFlag(IFlag<T> counter, LogicOperation operation, IFlag<T> compareCounter,
            string name) : base(name, false)
        {
            _counter = counter;
            _operation = operation;
            _compareCounter = compareCounter;
            _counter.AddListener(Update);
            _compareCounter.AddListener(Update);
            Update();
        }

        public static ChangingValueFlag<T> CreateFixed(IFlag<T> counter, LogicOperation operation,
            T compareValue, string name)
        {
            var compareCounter = new ChangingValueComparable<T>("fixed " + compareValue, compareValue);
            return new ChangingValueFlag<T>(counter, operation, compareCounter, name);
        }

        protected override bool NewValue()
        {
            return _operation.Operation(_counter.Value, _compareCounter.Value);
        }
    }
}