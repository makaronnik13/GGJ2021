using UnityEngine;

namespace com.armatur.common.unity
{
    public static class TransformEx {
        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform) {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }
    }
}