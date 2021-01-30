using System;
using System.Collections.Generic;
using System.Linq;
using com.armatur.common.flags;
using UnityEngine;
using UnityEngine.UI;

namespace com.armatur.common.unity
{
    [DisallowMultipleComponent]
    public class ButtonController : MonoBehaviour
    {
        private readonly List<Action> _actions = new List<Action>();
        private readonly FlagAll _visibleFlag = new FlagAll("Visible", true);
        private readonly FlagAll _enabledFlag  = new FlagAll("Enabled", true);

        protected virtual void Awake()
        {
            var component = GetComponent<Button>();
            if (component == null)
                throw new Exception("Button controller must be on button");
            component.onClick.AddListener(() => _actions.ToList().ForEach(a => a()));
            
            _visibleFlag.AddListener(b => gameObject.SetActive(b));
            _enabledFlag.AddListener(b =>
            {
                if (component != null)
                    component.interactable = b;
            });
        }

        public void AddEnabled(IFlag<bool> flag)
        {
            if (flag != null)
                _enabledFlag.AddFlag(flag);
        }

        public void AddVisible(IFlag<bool> flag)
        {
            if (flag != null)
                _visibleFlag.AddFlag(flag);
        }

        public void AddAction(Action action)
        {
            if (action != null)
                _actions.Add(action);
        }

        public void ClearAll()
        {
            _visibleFlag.RemoveAllFlags();
            _enabledFlag.RemoveAllFlags();
            _actions.Clear();
        }

        public void ClearActions()
        {
            _actions.Clear();
        }

        public void RemoveAction(Action action)
        {
            if (action != null)
                _actions.Remove(action);
        }

        public void AddActionController(ActionController actionController)
        {
            AddVisible(actionController.Active);
            AddEnabled(actionController.Allowed);
            AddAction(actionController.OnClick);
        }
        public void AddControls(IFlag<bool> visible, IFlag<bool> enabled, Action action)
        {
            AddVisible(visible);
            AddEnabled(enabled);
            AddAction(action);
        }

        public void TryInvoke()
        {
            if (_visibleFlag.Value && _enabledFlag.Value)
                _actions.ToList().ForEach(a => a());
        }
    }
}