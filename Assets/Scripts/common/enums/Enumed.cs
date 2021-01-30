using com.armatur.common.serialization;

namespace com.armatur.common.enums
{
    public class Enumed : IEnum
    {
        protected Enumed()
        {
        }

        public Enumed(string name)
        {
            Name = name;
        }

        [SerializeAsAttribute(FieldRequired.True)] public string Name { get; protected set; }

        public int Index { get; protected set; }

        string IEnum.Name()
        {
            return Name;
        }

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