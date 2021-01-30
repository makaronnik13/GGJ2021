namespace com.armatur.common.enums
{
    public class Indexed : IIndexed
    {
        public int Index { get; protected set; }

        int IIndexed.Index()
        {
            return Index;
        }

        void IIndexed.InitializeIndex(int value)
        {
            Index = value;
        }
    }
}