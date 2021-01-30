using System;
using System.Collections.Generic;

namespace com.armatur.common.events
{
    public class Priority
    {
        public const int High = int.MinValue;
        public const int AboveNormal = int.MinValue / 2;
        public const int Normal = 0;
        public const int BelowNormal = int.MaxValue / 2;
        public const int Low = int.MaxValue;
    }

    public class PriorityEvent : PrirorityList<PriorityEvent.ListenerController>
    {
        public readonly string Name;

        public PriorityEvent(string name)
        {
            Name = name;
        }

        public ListenerController AddListener(Action listener, int priority = 0)
        {
            return new ListenerController(this, listener, priority, true);
        }

        public ListenerController AddOneTimeListener(Action listener, int priority = 0)
        {
            return AddListener(listener, priority).OneTime();
        }

        public ListenerController AddAndStart(Action listener, int priority = 0)
        {
            listener();
            return AddListener(listener, priority);
        }

        public ListenerController CreateListener(Action listener, int priority = 0)
        {
            return new ListenerController(this, listener, priority, false);
        }

        public void RemoveListener(Action listener)
        {
            Remove(CreateListener(listener));
        }

        public void Fire()
        {
            var current = new List<ListenerController>(Listeners.Values);
            foreach (var t in current)
                t.Process();
        }

        public class ListenerController : IListenerController
        {
            private readonly PriorityEvent _event;
            private readonly Action _listener;
            private readonly int _priority;
            private bool _oneTime;

            public ListenerController(PriorityEvent ev, Action listener, int priority, bool add)
            {
                _event = ev;
                _listener = listener;
                _priority = priority;
                if (add)
                    Add();
            }

            public void Add()
            {
                _event.Add(this, _priority);
            }

            public void Remove()
            {
                _event.Remove(this);
            }

            public ListenerController OneTime()
            {
                _oneTime = true;
                return this;
            }

            public void Process()
            {
                _listener();
                if (_oneTime)
                    Remove();
            }

            private bool Equals(ListenerController other)
            {
                return Equals(_listener, other._listener);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((ListenerController) obj);
            }

            public override int GetHashCode()
            {
                return _listener != null ? _listener.GetHashCode() : 0;
            }
        }
    }

    public interface IListenerController
    {
        void Add();
        void Remove();
    }

    public class PriorityEvent<T> : PrirorityList<PriorityEvent<T>.ListenerController> 
    {
        public readonly string Name;

        public PriorityEvent(string name)
        {
            Name = name;
        }

        public ListenerController AddListener(Action<T> listener, int priority = 0)
        {
            return new ListenerController(this, listener, priority, true);
        }

        public ListenerController AddOneTimeListener(Action<T> listener, int priority = 0)
        {
            return AddListener(listener, priority).OneTime();
        }


        public ListenerController CreateListener(Action<T> listener, int priority = 0)
        {
            return new ListenerController(this, listener, priority, false);
        }

        public ListenerController CreateOneTimeListener(Action<T> listener, int priority = 0)
        {
            return CreateListener(listener, priority).OneTime();
        }

        public ListenerController AddListener(Action listener, int priority = 0)
        {
            return new ListenerController(this, listener, priority, true);
        }

        public ListenerController AddOneTimeListener(Action listener, int priority = 0)
        {
            return AddListener(listener, priority).OneTime();
        }


        public ListenerController CreateListener(Action listener, int priority = 0)
        {
            return new ListenerController(this, listener, priority, false);
        }

        public ListenerController CreateOneTimeListener(Action listener, int priority = 0)
        {
            return CreateListener(listener, priority).OneTime();
        }

        public void RemoveListener(Action<T> listener)
        {
            Remove(CreateListener(listener));
        }

        public void RemoveListener(Action listener)
        {
            Remove(CreateListener(listener));
        }

        public void Fire(T arg)
        {
            var current = new List<ListenerController>(Listeners.Values);
            foreach (var t in current)
                t.Process(arg);
        }

        public void ClearListeners()
        {
            Listeners.Clear();
        }

        public class ListenerController : IListenerController
        {
            private readonly PriorityEvent<T> _event;
            private readonly Action<T> _listener;
            private readonly Action _simpleListener;
            private readonly int _priority;
            private bool _oneTime;

            public ListenerController(PriorityEvent<T> ev, Action<T> listener, int priority, bool add)
            {
                _event = ev;
                _listener = listener;
                _priority = priority;
                if (add)
                    Add();
            }

            public ListenerController(PriorityEvent<T> ev, Action listener, int priority, bool add)
            {
                _event = ev;
                _simpleListener = listener;
                _priority = priority;
                if (add)
                    Add();
            }

            public void Add()
            {
                _event.Add(this, _priority);
            }

            public void Remove()
            {
                _event.Remove(this);
            }

            public ListenerController OneTime()
            {
                _oneTime = true;
                return this;
            }

            public void Process(T obj)
            {
                _listener?.Invoke(obj);
                _simpleListener?.Invoke();
                if (_oneTime)
                    Remove();
            }

            private bool Equals(ListenerController other)
            {
                return Equals(_listener, other._listener) && Equals(_simpleListener, other._simpleListener);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((ListenerController) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_listener != null ? _listener.GetHashCode() : 0) * 397) ^ (_simpleListener != null ? _simpleListener.GetHashCode() : 0);
                }
            }
        }
    }
}