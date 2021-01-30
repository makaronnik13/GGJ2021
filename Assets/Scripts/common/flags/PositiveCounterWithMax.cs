namespace com.armatur.common.flags
{
    public class PositiveCounterWithMax : CounterWithMax
    {
        public PositiveCounterWithMax(string name) : base(name)
        {
            CurrentCounter.SetMinimal(0);
            MaxCounter.SetMinimal(0);
        }
    }
}