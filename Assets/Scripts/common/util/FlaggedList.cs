using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using com.armatur.common.flags;
using com.armatur.common.save;
using com.armatur.common.serialization;

namespace com.armatur.common.util
{
    public class FlaggedList<T> : List<T>, IFlaggedReadOnlyList<T>
    {
        [Savable]
        [SerializeData("SizeFlag", FieldRequired.False)]
        public IntFlag SizeFlag { get; set; } = new IntFlag("FlaggedList size");

        public FlaggedList()
        {
            this.Items = new List<T>();
            UpdateCounter();
        }

        public FlaggedList(List<T> items)
        {
            this.Items = items;
            UpdateCounter();
        }

        public ReadOnlyCollection<T> ReadOnly { get; private set; }

        public List<T> Items
        {
            get
            {
                return base.FindAll((i) => i != null);
            }
            set
            {
                base.Clear();
                base.AddRange(value);
            }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public new void Add(T item)
        {
            base.Add(item);
            UpdateCounter();
        }

        public new void AddRange(IEnumerable<T> items)
        {
            base.AddRange(items);
            UpdateCounter();
        }

        public new void Clear()
        {
            base.Clear();
            UpdateCounter();
        }

        public new bool Contains(T item)
        {
            return base.Contains(item);
        }

        public new void CopyTo(T[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
            UpdateCounter();
        }

        public new bool Remove(T item)
        {
            var remove = base.Remove(item);
            if (remove)
                UpdateCounter();
            return remove;
        }

        public new int Count => base.Count;

        public bool IsReadOnly => false;

        public new int IndexOf(T item)
        {
            return base.IndexOf(item);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            UpdateCounter();
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            UpdateCounter();
        }

        public new T this[int index]
        {
            get { return base[index]; }
            set
            {
                base[index] = value;
                UpdateCounter();
            }
        }

        private void UpdateCounter()
        {
            SizeFlag.State = base.Count;
            ReadOnly = base.AsReadOnly();
        }


        public int Size()
        {
            return SizeFlag.Value;
        }

        [OnDeserialize]
        public void OnDeserialize(SaveProcessor processor)
        {
            SizeFlag.State = Items.Count();
        }
    }

    public static class FlaggedListExt
    {
        public static FlaggedList<T> ToFlaggedList<T>(this IEnumerable<T> source)
        {
            return new FlaggedList<T>(source.ToList());
        }
    }
}