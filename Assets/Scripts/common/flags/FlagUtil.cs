namespace com.armatur.common.flags
{
    public static class FlagUtil
    {
        private abstract class ValueFlag<T> : FlagUpdater<bool>
        {
            protected readonly IFlag<T> _flag;

            protected ValueFlag(IFlag<T> flag) : base(flag.Name + " null", false)
            {
                _flag = flag;
                _flag.AddListener(Update);
                Update();
            }
           
        }

        private class ValueNullFlag<T> : ValueFlag<T>
        {
            public ValueNullFlag(IFlag<T> flag) : base(flag)
            {
            }

            protected override bool NewValue()
            {
                return _flag.Value == null;
            }
        }


        private class ValueNotNullFlag<T> : ValueFlag<T>
        {
            public ValueNotNullFlag(IFlag<T> flag) : base(flag)
            {
            }

            protected override bool NewValue()
            {
                return _flag.Value != null;
            }
        }
        
        public static FlagUpdater<bool> GetNullFlag<T>(this ChangingValue<T> flag)
        {
            return new ValueNullFlag<T>(flag);
        }
        
        public static FlagUpdater<bool> GetNotNullFlag<T>(this ChangingValue<T> flag)
        {
            return new ValueNotNullFlag<T>(flag);
        }
    }
}