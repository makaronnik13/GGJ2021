using System.Collections.Generic;
using com.armatur.common.serialization;
using UnityEngine;
using UnityEngine.UI;

namespace com.armatur.common.unity.serialization
{
    public class CollectionView : MonoBehaviour, IMemberView
    {
        private TypeWrapper _elementTypeWrapper;
        private SaveProcessorView _view;
        private readonly List<IMemberView> _member = new List<IMemberView>();
        [SerializeField] private GameObject _listArea;
        [SerializeField] private Button _addItemButton;
        [SerializeField] private CollectionItemView _collectionItemViewPrefab;

        protected virtual void Awake()
        {
            _addItemButton.onClick.AddListener(() =>
            {
                if (_elementTypeWrapper != null)
                    AddItem();
            });
        }

        public void Init(TypeWrapper elementTypeWrapper, SaveProcessorView view)
        {
            _elementTypeWrapper = elementTypeWrapper;
            _view = view;
        }

        public string Name { get; }

        public IMemberView AddLevel(string levelName, bool add)
        {
            var wrapper = _elementTypeWrapper.GetPolymorphicTypeWrapper(levelName);
            if (wrapper == null) return null;
            if (add)
            {
                return AddItem()?.AddLevel(levelName, true);
            }

            return null;
//                return _member.ElementAtOrDefault(testIndex++)?.AddLevel(levelName, false);

        }

        private IMemberView AddItem()
        {
            var memberView = _view.CreateView(null, _elementTypeWrapper, true);
            var collectionItemView = Instantiate(_collectionItemViewPrefab);
            ((MonoBehaviour) memberView).gameObject.transform.SetParent(collectionItemView.Area.transform, false);
            collectionItemView.gameObject.transform.SetParent(_listArea.transform, false);
            _member.Add(memberView);
            collectionItemView.CloseButton.onClick.AddListener(() => RemoveItem(collectionItemView.gameObject, memberView));
            return memberView;
        }

        private void RemoveItem(GameObject gameObject, IMemberView memberView)
        {
            _member.Remove(memberView);
            Destroy(gameObject);
        }

        public IMemberView RemoveLevel()
        {
            return null;
        }

        public IEnumerable<IMemberView> GetCollectionLevels()
        {
            return _member;
        }

        public void SetValue(string value)
        {
            throw new System.NotImplementedException();
        }

        public string GetValue()
        {
            throw new System.NotImplementedException();
        }
    }
}