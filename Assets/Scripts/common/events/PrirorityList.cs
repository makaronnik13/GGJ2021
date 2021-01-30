using System;
using System.Collections.Generic;

namespace com.armatur.common.events
{
    internal class DuplicateKeyComparer<TKey>
        : IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            var result = x.CompareTo(y);

            return result == 0 ? 1 : result;
        }

        #endregion
    }

    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return -y.CompareTo(x);
        }
    }

    public class PrirorityList<T>
    {
        protected readonly SortedList<int, T> Listeners =
            new SortedList<int, T>(new DuplicateKeyComparer<int>());

        public IList<T> Values => new List<T>(Listeners.Values);
        public int Count => Listeners.Count;

        protected void Add(T listener, int priority = 0)
        {
            Listeners.Add(priority, listener);
        }

        protected void Remove(T listener)
        {
            var index = Listeners.IndexOfValue(listener);
            if (-1 != index)
                Listeners.RemoveAt(index);
        }

        protected T Pop()
        {
            var res = Listeners.Values[0];
            Listeners.RemoveAt(0);
            return res;
        }
    }
}