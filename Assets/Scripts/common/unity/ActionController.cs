using System;
using com.armatur.common.flags;

namespace com.armatur.common.unity
{
    public class ActionController
    {
        public readonly IFlag<bool> Active;
        public readonly IFlag<bool> Allowed;
        public readonly Action OnClick;

        public ActionController(IFlag<bool> active, IFlag<bool> allowed, Action onClick)
        {
            Active = active;
            Allowed = allowed;
            OnClick = onClick;
        }

        public ActionController(IFlag<bool> active, Action onClick)
        {
            Active = active;
            OnClick = onClick;
        }

    }
}