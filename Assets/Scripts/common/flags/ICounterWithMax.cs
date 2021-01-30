namespace com.armatur.common.flags
{
    public interface ICounterWithMax
    {
        Counter Current { get; }
        Counter Max { get; }
        Counter Missed { get; }

        int CurrentValue  { get; }
        int MaxValue  { get; }
        int MissedValue  { get; }
    }
}