using System.Collections.ObjectModel;
using System.Linq;
using com.armatur.common.save;
using com.armatur.common.util;
using com.armatur.common.serialization;

namespace com.armatur.common.enums
{
    public class EnumPool<T> : ISerializerStringConverter where T : class, IEnum
    {
        [SerializeCollection(FieldOmitName.True)] private readonly FlaggedList<T> _enums = new FlaggedList<T>();

        public ReadOnlyCollection<T> Values => _enums.ReadOnly;

        public object ConvertFromString(string xml)
        {
            return GetByName(xml);
        }

        public string ConvertToString(object obj)
        {
            return (obj as T)?.Name();
        }

        [OnDeserialize]
        public void OnDeserialize(SaveProcessor processor)
        {
            for (var i = 0; i < _enums.Count; i++)
                _enums[i].InitializeIndex(i);
            processor.SetConverter(typeof(T), this);
        }

        public void AddEnum(T en)
        {
            en.InitializeIndex(_enums.Count);
            _enums.Add(en);
        }

        public T GetByName(string name)
        {
            return _enums.FirstOrDefault(enumExample => enumExample.Name().Equals(name));
        }
    }
}