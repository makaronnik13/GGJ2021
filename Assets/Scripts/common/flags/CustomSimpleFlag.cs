using System;

namespace com.armatur.common.flags
{
    public class CustomSimpleFlag : FlagUpdater<bool>
    {
        private Func<bool> _updater = null;

        public CustomSimpleFlag(string name, Func<bool> updater, bool initialValue) : base(name, initialValue)
        {
            SetUpdater(updater);
        }

        public void SetUpdater(Func<bool> updater)
        {
            _updater = updater;
            Update();
        }

        public new void Update()
        {
            base.Update();
        }

        protected override bool NewValue()
        {
            return _updater == null ? false : _updater();
        }
    }
}