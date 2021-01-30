using com.armatur.common.save;
using com.armatur.common.util;
using com.armatur.common.serialization;

namespace com.armatur.common.enums
{
    public class IndexedPool<T> : ISerializerStringConverter where T : IIndexed
    {
        [SerializeCollection(FieldOmitName.True)] private readonly FlaggedList<T> _enums = new FlaggedList<T>();

        public IFlaggedReadOnlyList<T> Values => _enums;

        public object ConvertFromString(string xml)
        {
            return GetByIndex(int.Parse(xml));
        }

        public string ConvertToString(object obj)
        {
            if (obj is T)
                return ((T) obj).Index().ToString();
            return null;
        }

        [OnDeserialize]
        public void OnDeserialize(SaveProcessor processor)
        {
            for (var i = 0; i < _enums.Count; i++)
                _enums[i].InitializeIndex(i);
            processor.SetConverter(typeof(T), this);
        }

        public void AddIndexed(T en)
        {
            en.InitializeIndex(_enums.Count);
            _enums.Add(en);
        }

        public T GetByIndex(int index)
        {
            return _enums[index];
        }
    }
}