using UnityEngine;
using UnityEngine.UI;

namespace com.armatur.common.unity
{
    public class HorizontalLayout
    {
        public static GameObject CreateHorizontalLayout()
        {
            var res = new GameObject();
            res.AddComponent<HorizontalLayoutGroup>();
            var fitter = res.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return res;
        }
    }
}