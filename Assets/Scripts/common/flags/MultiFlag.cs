using System.Collections.Generic;
using System.Linq;
using com.armatur.common.serialization;

namespace com.armatur.common.flags
{
    public class FlagAll : MultiFlag
    {
        private FlagAll() : base(null, false)
        {
        }

        public FlagAll(string name, bool initialValue = false) : base(name, initialValue)
        {
        }

        protected override bool MultiFlagValue()
        {
            return Flags.Aggregate(true, (current, flag) => current && flag.Value());
        }
    }

    public class FlagAny : MultiFlag
    {
        private FlagAny() : base(null, false)
        {
        }

        public FlagAny(string name, bool initialValue = false) : base(name, initialValue)
        {
        }

        protected override bool MultiFlagValue()
        {
            return Flags.Aggregate(false, (current, flag) => current || flag.Value());
        }
    }

    public abstract class MultiFlag : FlagUpdater<bool>
    {
        [SerializeAsAttribute(FieldRequired.False)]
        private readonly bool _initialValue;
        protected List<InvertableFlag> Flags = new List<InvertableFlag>();

        protected MultiFlag(string name, bool initialValue) : base(name, initialValue)
        {
            _initialValue = initialValue;
            Update();
        }

        public SimpleFlag AddNewFlag(string name, bool value = false, bool reverse = false)
        {
            var flag = new SimpleFlag(name, value);
            AddFlag(flag, reverse);
            return flag;
        }

        public MultiFlag AddFlag(IFlag<bool> flag, bool reverse = false)
        {
            Flags.Add(new InvertableFlag(flag, reverse));
            flag.AddListener(Update, false);

            // Force update of listeners in case it was first flag - set it to !NewValue(), and then in update to NewValue() - and notify all the users.
            if (Flags.Count == 1)
                SetInnerState(!NewValue(), false);
            Update();
            return this;
        }

        public void RemoveFlag(IFlag<bool> flag)
        {
            for (var i = 0; i < Flags.Count; ++i)
            {
                if (Flags[i].Flag != flag) continue;
                flag.RemoveListener(Update);
                Flags.RemoveAt(i);
                Update();
                break;
            }
        }

        public void RemoveAllFlags()
        {
            foreach (var t in Flags)
                t.Flag.RemoveListener(Update);
            Flags = new List<InvertableFlag>();
        }

        protected override bool NewValue()
        {
            return Flags.Count == 0 ? _initialValue : MultiFlagValue();
        }

        protected abstract bool MultiFlagValue();

        protected class InvertableFlag
        {
            private readonly bool _invert;
            public readonly IFlag<bool> Flag;

            public InvertableFlag(IFlag<bool> flag, bool invert)
            {
                Flag = flag;
                _invert = invert;
            }

            public bool Value()
            {
                return _invert ? !Flag.Value : Flag.Value;
            }
        }
    }
}