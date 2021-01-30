using System;
using System.Collections.Generic;
using System.Linq;
using com.armatur.common.serialization;
using UnityEngine;
using UnityEngine.UI;

namespace com.armatur.common.unity.serialization
{
    public class ComplexMemberUnityView : FoldoutComponent, IMemberView
    {
        private readonly List<IMemberView> _member = new List<IMemberView>();
        private SaveProcessorView _view;
        private bool _inheritAllowed;
        private bool _inheritLevel;
        private bool _nameLevel;
        private TypeWrapper _initialTypeWrapper;
        private TypeWrapper _currentTypeWrapper;

        [SerializeField] private Dropdown _inheritTypeView;
        private readonly List<string> _availableTypes = new List<string>();
        [SerializeField] private CollectionView _collectionViewPrefab;
        private CollectionView _collectionView;
        private string _name;

        protected override void Awake()
        {
            base.Awake();
            _inheritTypeView.onValueChanged.AddListener(index =>
            {
                var selectedType = _initialTypeWrapper.AvailableTypes.FirstOrDefault(s => s.Equals(_availableTypes.ElementAtOrDefault(index)));
                SetCurrentTypeWrapper(selectedType != null ? _initialTypeWrapper.GetPolymorphicTypeWrapper(selectedType) : null);
            });
        }

        public void Init(string memberName, TypeWrapper typeWrapper, SaveProcessorView view, bool allowInheritance)
        {
            _name = memberName;
            _titleField.text = memberName ?? typeWrapper.Name();
            _view = view;
            _initialTypeWrapper = typeWrapper;
            _nameLevel = false;
            if (_initialTypeWrapper.AvailableTypes.Count() > 1 && allowInheritance)
            {
                _inheritTypeView.gameObject.SetActive(true);
                _inheritAllowed = true;
                _inheritTypeView.ClearOptions();
                _availableTypes.Clear();
                _availableTypes.Add("Null");
                _availableTypes.AddRange(_initialTypeWrapper.AvailableTypes);
                _inheritTypeView.AddOptions(_availableTypes);
                SetCurrentTypeWrapper(null);
            }
            else
            {
                if (_name == null)
                {
                    if (allowInheritance)
                    {
                        _name = _initialTypeWrapper.Name();
                    }
                    else
                    {
                        _foldoutState.State = true;
                        _clickableArea.gameObject.SetActive(false);
                    }
                }


                _inheritTypeView.gameObject.SetActive(false);
                _inheritAllowed = false;
                SetCurrentTypeWrapper(_initialTypeWrapper);
            }
        }

        public void Init(MemberWrapper memberWrapper, SaveProcessorView view)
        {
            Init(memberWrapper.OmitOuterName ? null : memberWrapper.Name, memberWrapper.TypeWrapper, view, memberWrapper.AllowInheritance);
        }

        public void SetCurrentTypeWrapper(TypeWrapper wrapper)
        {
            if (_currentTypeWrapper == wrapper) return;
            _currentTypeWrapper = wrapper;

            if (_inheritAllowed)
                _inheritTypeView.value = wrapper != null ? _availableTypes.IndexOf(wrapper.Name()) : 0;
            _inheritLevel = false;
            _member.ForEach(view => Destroy(((MonoBehaviour) view).gameObject));
            _member.Clear();
            if (_collectionView != null)
            {
                Destroy(_collectionView.gameObject);
                _collectionView = null;
            }

            var collectionTypeWrapper = _currentTypeWrapper as CollectionTypeWrapper;
            if (collectionTypeWrapper != null)
            {
                _collectionView = Instantiate(_collectionViewPrefab);
                _collectionView.Init(collectionTypeWrapper.ElementTypeWrapper, _view);
                _collectionView.transform.SetParent(_collapsedArea.transform);
            }

            var typeWrapper = _currentTypeWrapper as ComplexTypeWrapper;
            typeWrapper?.Members.ForEach(memberWrapper =>
            {
                var memberView = _view.CreateView(memberWrapper);
                ((MonoBehaviour) memberView).gameObject.transform.SetParent(_collapsedArea.transform, false);
                _member.Add(memberView);
            });
        }


        public IEnumerable<IMemberView> GetCollectionLevels()
        {
            if (_name == null || _nameLevel)
            {
                if (_inheritAllowed && !_inheritLevel) return new[] {this};
                var res = _member.SelectMany(view => view.GetCollectionLevels());
                if (_collectionView)
                    res = res.Concat(_collectionView.GetCollectionLevels());
                return res;
            }
            return Enumerable.Empty<IMemberView>();
        }

        public string Name
        {
            get
            {
                if (_name != null && !_nameLevel) return _name;
                return _inheritAllowed ? _currentTypeWrapper?.Name() : null;
            }
        }

        public virtual IMemberView AddLevel(string levelName, bool add)
        {
            if (_name != null && !_nameLevel)
            {
                if (!_name.Equals(levelName)) return null;
                _nameLevel = true;
                return this;
            }

            if (!_inheritAllowed || _inheritLevel)
            {
                IMemberView res = null;
                if (_collectionView != null)
                    res = _collectionView.AddLevel(levelName, add);
                if (res != null)
                    return res;
                try
                {
                    foreach (var view in _member)
                    {
                        var memberView = view.AddLevel(levelName, add);
                        if (memberView != null)
                            return memberView;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (add)
            {
                var polymorphicTypeWrapper = _initialTypeWrapper.GetPolymorphicTypeWrapper(levelName);
                if (polymorphicTypeWrapper == null) return null;
                SetCurrentTypeWrapper(polymorphicTypeWrapper);
            }
            else
            {
                if (_currentTypeWrapper == null || !_currentTypeWrapper.Name().Equals(levelName))
                    return null;
            }

            _inheritLevel = true;
            return this;
        }

        public IMemberView RemoveLevel()
        {
            if (_inheritLevel)
            {
                _inheritLevel = false;
                return null;
            }

            if (_name != null && _nameLevel)
            {
                _nameLevel = false;
                return null;
            }

            return null;
        }

         public override string ToString()
        {
            return (_name ?? "") + " " + _initialTypeWrapper.Name();
        }

      public virtual void SetValue(string value)
        {
            throw new System.NotImplementedException();
        }

        public virtual string GetValue()
        {
            throw new System.NotImplementedException();
        }
    }
}