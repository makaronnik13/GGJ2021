using com.armatur.common.flags;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.armatur.common.unity
{
    public class FoldoutComponent : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _titleField;
        [SerializeField] protected Button _clickableArea;
        [SerializeField] protected GameObject _openedImage;
        [SerializeField] protected GameObject _hidedImage;
        [SerializeField] protected GameObject _collapsedArea;

        protected readonly SimpleFlag _foldoutState = new SimpleFlag(null, false);

        protected virtual void Awake()
        {
            _clickableArea.onClick.AddListener(_foldoutState.Reverse);
            _foldoutState.AddListener(b =>
            {
                _collapsedArea.SetActive(b);
                _openedImage.SetActive(b);
                _hidedImage.SetActive(!b);
//                var par = gameObject;
 /*               while (null != par.transform.parent)
                {
                    par = par.transform.parent.gameObject;
                    if (par.GetComponent<LayoutGroup>())
                        LayoutRebuilder.MarkLayoutForRebuild(par.GetComponent<RectTransform>());
                }*/
            });
        }
    }
}