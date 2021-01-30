using System;
using System.Collections.Generic;
using com.armatur.common.enums;

namespace com.armatur.common.events
{
    public class EnumEventDispatcher<TEn, T> where TEn : IEnum
    {
        private readonly List<PriorityEvent<T>> _events;

        public EnumEventDispatcher(IList<TEn> enums)
        {
            _events = new List<PriorityEvent<T>>(enums.Count);
            for (var i = 0; i < enums.Count; ++i)
                _events[i] = new PriorityEvent<T>(enums[i].Name());
        }

        public void AddListener(TEn en, Action<T> listener, int priority)
        {
            _events[en.Index()].AddListener(listener, priority);
        }

        public void RemoveListener(TEn en, Action<T> listener)
        {
            _events[en.Index()].RemoveListener(listener);
        }

        public void Fire(TEn en, T arg)
        {
            _events[en.Index()].Fire(arg);
        }
    }
}