using com.armatur.common.save;

namespace com.armatur.common.serialization
{
    public class ObjectWrapper : ISavable
    {
        private readonly TypeWrapper _typeWrapper;
        private object _object;

        public ObjectWrapper(object o)
        {
            _object = o;
            _typeWrapper = TypeWrapperPool.Instance.GetWrapper(o.GetType());
        }

        public void Save(SaveProcessor processor)
        {
            _typeWrapper.Save(processor, _object);
        }

        public bool Load(SaveProcessor processor)
        {
            return _typeWrapper.Load(processor, ref _object);
        }

        public string Name()
        {
            return _typeWrapper.Name();
        }
    }
}