using UnityEngine;

namespace com.armatur.common.unity
{
    [DisallowMultipleComponent]
    public class LockedPosition : MonoBehaviour
    {
        private Vector3 _position;
        private RectTransform _rectTransform;

        // Use this for initialization
        private void Start()
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _position = _rectTransform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_position != _rectTransform.position)
                _rectTransform.position = _position;
        }
    }
}