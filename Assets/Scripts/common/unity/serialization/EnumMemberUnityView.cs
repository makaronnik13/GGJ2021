using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.armatur.common.unity.serialization
{
    public class EnumMemberUnityView : MonoBehaviour, IMemberView
    {
        [SerializeField] private TextMeshProUGUI _titleField;
        [SerializeField] private Dropdown _dropdown;
        private List<string> _options;

        public void Init(string memberName, Type type)
        {
            Name = memberName;
            _titleField.text = memberName;
            _dropdown.ClearOptions();
            _options = Enum.GetValues(type).Cast<Enum>().Select(en => en.ToString()).ToList();
            _dropdown.AddOptions(_options);
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
            _dropdown.value = _options.IndexOf(value);
        }

        public string GetValue()
        {
            return _options.ElementAtOrDefault(_dropdown.value);
        }
    }
}