using System;
using System.Collections.Generic;
using com.armatur.common.events;
using com.armatur.common.serialization;

namespace com.armatur.common.save
{
    public abstract class SaveProcessor
    {
        private readonly List<int> _levelsInTransaction = new List<int>();
        private List<Dictionary<string, int>> _levels = new List<Dictionary<string, int>> {new Dictionary<string, int>()};

        private readonly Dictionary<Type, ISerializerStringConverter> _stringConverters =
            new Dictionary<Type, ISerializerStringConverter>();

        protected SaveLoadParams @params = null;

        public bool IsSavableOnly
        {
            get {
                if (@params == null)
                    return false;

                return @params.savableOnly;
            }
        }

        public bool SkipSavables
        {
            get
            {
                if (@params == null)
                    return false;

                return @params.skipSavables;
            }
        }

        public readonly PriorityEvent OnLoadEvent = new PriorityEvent("Processor loaded");

        public abstract void AddField(string name, string value);
        public abstract string GetField(string name);
        public abstract void AddData(string value);
        public abstract string GetData(string name = null);
        public abstract void ForEachLevel(Action<string> action);
        public abstract bool AddLevel(string name, bool omited, bool add);
        public abstract void RemoveOneLevel(bool omited);

        public bool HasConverter(Type type)
        {
            return null != GetConverter(type);
        }

        public ISerializerStringConverter GetConverter(Type type)
        {
            return _stringConverters[type];
        }

        public void SetConverter(Type type, ISerializerStringConverter converter)
        {
            _stringConverters[type] = converter;
            TypeWrapperPool.Instance.GetWrapper(type).Converter = converter;
        }

        public string GetAnyValue(string name)
        {
            return GetField(name) ?? GetData(name);
        }

        public virtual void SaveObject(object o)
        {
            Save(new ObjectWrapper(o));
        }

        public T LoadObject<T>(T obj = null) where T: class
        {
            try
            {
                if (obj == null)
                {
                    var typeWrapper = TypeWrapperPool.Instance.GetWrapper(typeof(T));
                    var o = typeWrapper.CreateDefault();
                    Load(new ObjectWrapper(o));
                    return (T)o;
                }
                else
                {
                    var typeWrapper = TypeWrapperPool.Instance.GetWrapper(typeof(T));
                    Load(new ObjectWrapper(obj));
                    return obj;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Save(ISavable savable)
        {
            if (null != savable.Name())
                AddLevel(savable.Name(), false, true);
            savable.Save(this);
            if (null != savable.Name())
                RemoveOneLevel(false);
        }

        public bool Load(ISavable savable)
        {
            var level = false;
            if (null != savable.Name())
                level = AddLevel(savable.Name(), false, false);
            var res = savable.Load(this);
            if (null != savable.Name() && level)
                RemoveOneLevel(false);
            return res;
        }

        protected int AddLevelToStructure(string name)
        {
            if (_levelsInTransaction.Count > 0)
                ++_levelsInTransaction[_levelsInTransaction.Count - 1];
            int value;
            value = _levels[_levels.Count - 1].TryGetValue(name, out value) ? value : -1;
            ++value;
            _levels[_levels.Count - 1][name] = value;
            _levels.Add(new Dictionary<string, int>());
            return value;
        }

        protected void RemoveLevelFromStructure()
        {
            _levels.RemoveAt(_levels.Count - 1);
            if (_levelsInTransaction.Count > 0)
                --_levelsInTransaction[_levelsInTransaction.Count - 1];
        }

        public void BeginTransaction()
        {
            _levelsInTransaction.Add(0);
        }

        public void EndTransaction()
        {
            if (_levelsInTransaction.Count <= 0) return;
            while (_levelsInTransaction[_levelsInTransaction.Count - 1] > 0)
                RemoveOneLevel(false);
            _levelsInTransaction.RemoveAt(_levelsInTransaction.Count - 1);
        }

        public void ResetData()
        {
            _levels = new List<Dictionary<string, int>> {new Dictionary<string, int>()};
        }
    }
}