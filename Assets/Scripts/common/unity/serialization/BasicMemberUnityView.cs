using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.armatur.common.unity.serialization
{
    public class BasicMemberUnityView : MonoBehaviour, IMemberView
    {
        [SerializeField] private TextMeshProUGUI _titleField;
        [SerializeField] private InputField _inputField;

        public void Init(string memberName)
        {
            Name = memberName;
            _titleField.text = memberName;
        }

        public string Name { get; private set; }

        public IMemberView AddLevel(string levelName, bool add)
        {
            return Name.Equals(levelName) ? this : null;
        }

        public IMemberView RemoveLevel()
        {
            return null;
        }

        public IEnumerable<IMemberView> GetCollectionLevels()
        {
            return Enumerable.Empty<IMemberView>();
        }

        public void SetValue(string value)
        {
            _inputField.text = value;
        }

        public string GetValue()
        {
            return _inputField.text;
        }
    }
}