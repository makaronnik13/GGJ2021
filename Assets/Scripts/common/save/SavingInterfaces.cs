namespace com.armatur.common.save
{
    public interface ISavable
    {
        void Save(SaveProcessor processor);
        bool Load(SaveProcessor processor);
        string Name();
    }

    public interface IMappableObject
    {
        void MapData();
    }

    public interface ILoadable
    {
        void OnLoad();
    }
}