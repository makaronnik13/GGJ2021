using System.Collections.Generic;

namespace com.armatur.common.unity.serialization
{
    public interface IMemberView
    {
        string Name { get; }
        IMemberView AddLevel(string levelName, bool add);
        IMemberView RemoveLevel();
        IEnumerable<IMemberView> GetCollectionLevels();
        void SetValue(string value);
        string GetValue();
    }
}