using System;
using com.armatur.common.events;
using com.armatur.common.logic;

namespace com.armatur.common.flags
{
    public interface IFlag<out T>
    {
        string Name { get; }
        T Value { get; }
        IListenerController AddListener(Action<T> listener, bool run = true, int priority = 0);
        void RemoveListener(Action<T> listener);
    }

    public interface IFlagComparable<T> : IFlag<T> where T : IComparable
    {
        IFlag<bool> GetFlag(LogicOperation operation, IFlag<T> compareCounter, string name = null);

        IFlag<bool> GetFixedFlag(LogicOperation operation, T compareValue, string name = null);
    }
    
}