using System.Collections.Generic;
using System.Linq;

namespace com.armatur.common.flags
{
    public class SumFlag : FlagUpdater<double>

    {
        private readonly List<ChangingValue<double>> _flags = new List<ChangingValue<double>>();

        public SumFlag(string name, List<ChangingValue<double>> flags = null) : base(name, default(double))
        {
            if (flags != null)
                _flags = flags;

            Update();
        }

        protected override double NewValue()
        {
            return _flags.Sum(f => f.Value);
        }
        
        public void AddElement(ChangingValue<double> flag)
        {
            flag.AddListener(ValueUpdatedHandler, false);
            _flags.Add(flag);
            Update();
        }

        public void RemoveElement(ChangingValue<double> flag)
        {
            flag.RemoveListener(ValueUpdatedHandler);
            _flags.Remove(flag);
            Update();
        }

        private void ValueUpdatedHandler(double value)
        {

        }

        public double Sum()
        {
            Update();
            return Value;
        }

        public void Update()
        {
            base.Update();
        }
    }
}