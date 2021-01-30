using com.armatur.common.save;
using com.armatur.common.serialization;

namespace com.armatur.common.flags
{
    public class CounterWithMax : ICounterWithMax
    {
        protected readonly Counter CurrentCounter;
        protected readonly Counter MaxCounter;
        protected readonly Counter MissedCounter;

        public CounterWithMax(string name, int curr = 0, int max = 0)
        {
            CurrentCounter = new Counter(name + " current", curr);
            MaxCounter = new Counter(name + " max", max);
            MissedCounter = new Counter(name + " missed");
            UpdateMissed();
        }

        public Counter Current => CurrentCounter;
        public Counter Max => MaxCounter;
        public Counter Missed => MissedCounter;

        [Savable]
        [SerializeData("CurrentValue", FieldRequired.False)]
        public int CurrentValue
        {
            get { return CurrentCounter.Value; }
            set { CurrentCounter.SetValue(value); }
        }

        [Savable]
        [SerializeData("MaxValue", FieldRequired.False)]
        public int MaxValue
        {
            get { return MaxCounter.Value; }
            set { MaxCounter.SetValue(value); }
        }

        [Savable]
        [SerializeData("MissedValue", FieldRequired.False)]
        public int MissedValue
        {
            get { return MissedCounter.Value; }
            set { MissedCounter.SetValue(value); }
        }

        [OnDeserialize]
        public void OnDeserialize(SaveProcessor processor)
        {
            UpdateMissed();
        }

        [OnSerialize]
        public void OnSerialize(SaveProcessor processor)
        {
            UpdateMissed();
        }

        public int SetCurrent(int value)
        {
            return Change(value - CurrentCounter.Value);
        }

        public int Change(int value)
        {
            var overFull = CurrentValue + value - MaxValue;
            if (overFull > 0)
                value -= overFull;
            value = CurrentCounter.Change(value);
            UpdateMissed();
            return value;
        }

        public void SetMax(int value)
        {
            ChangeMax(value - MaxCounter.Value);
        }

        public int ChangeMax(int diff)
        {
            MaxCounter.Change(diff);
            var missed = MaxValue - CurrentValue;
            if (missed < 0)
                Change(missed);
            else
                UpdateMissed();

            return diff;
        }

        public void AddOne()
        {
            Change(1);
        }

        public void SubtractOne()
        {
            Change(-1);
        }

        public void AddOne<T>(T nonUsed)
        {
            AddOne();
        }

        public void SubtractOne<T>(T nonUsed)
        {
            SubtractOne();
        }

        private void UpdateMissed()
        {
            MissedCounter.SetValue(MaxValue - CurrentValue);
        }

        public void RaiseEvents()
        {
            CurrentCounter.RaiseEvent();
            MaxCounter.RaiseEvent();
            MissedCounter.RaiseEvent();
        }
    }
}