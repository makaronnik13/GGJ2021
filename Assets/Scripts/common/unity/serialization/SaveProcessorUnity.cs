using System;
using System.Collections.Generic;
using com.armatur.common.save;
using com.armatur.common.util;

namespace com.armatur.common.unity.serialization
{
    public class SaveProcessorUnity : SaveProcessor
    {
        private IMemberView _view;
        private IMemberView _rootView;
        private Stack<IMemberView> _viewList = new Stack<IMemberView>();

        public void SetRootView(IMemberView rootView)
        {
            _rootView = rootView;
        }

        public override void AddField(string name, string value)
        {
            if (!AddLevel(name, false, true)) return;
            _view.SetValue(value);
            RemoveOneLevel(false);
        }

        public override string GetField(string name)
        {
            if (!AddLevel(name, false, false)) return null;
            var value = _view.GetValue();
            RemoveOneLevel(false);
            return value;
        }

        public override void AddData(string value)
        {
            try
            {
                _view.SetValue(value);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override string GetData(string name = null)
        {
            return _view.GetValue();
        }

        public override void ForEachLevel(Action<string> action)
        {
            var memberView = _view;
            memberView.GetCollectionLevels().ForEach(view =>
            {
                var name = view.Name;
                if (name == null)
                    return;
                _view = view;
                var res = AddLevel(name, false, false);
                action(name);
                RemoveOneLevel(false);
            });
            _view = memberView;
        }

        public override bool AddLevel(string name, bool omited, bool add)
        {
            if (omited)
                return true;
//            var value = AddLevelToStructure(name);
            var res = false;
            if (_view == null)
            {
                _view = _rootView.AddLevel(name, add);
                res = _view != null;
            }
            else
            {
                var newView = _view.AddLevel(name, add);
                if (newView != null)
                {
                    _viewList.Push(_view);
                    _view = newView;
                    res = true;
                }
            }
//            if (!res)
//                RemoveLevelFromStructure();

            return res;
        }

        public override void RemoveOneLevel(bool omited)
        {
            if (omited) return;
            var removeLevel = _view?.RemoveLevel();
            if (removeLevel != null)
            {
                _view = removeLevel;
            }
            else
            {
                _view = _viewList.Count > 0 ? _viewList.Pop() : null;
            }
//            RemoveLevelFromStructure();
        }
    }
}