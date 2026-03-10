using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIManager
{
    public class UIScreen<T> : BaseUIScreen where T : UIScreen<T>
    {
        public event Action<T> onShowEnded;
        public event Action<T> onShowStarted;
        public event Action<T> onHideStarted;
        public event Action<T> onHideEnded;

        public override void OnShow()
        {
            base.OnShow();
            onShowStarted?.Invoke(this as T);
        }

        public override void OnHide()
        {
            base.OnHide();
            onHideStarted?.Invoke(this as T);
        }

        protected override void OnShowTransitionCompleted()
        {
            base.OnShowTransitionCompleted();
            onShowEnded?.Invoke(this as T);
        }

        protected override void OnHideTransitionCompleted()
        {
            base.OnHideTransitionCompleted();
            onHideEnded?.Invoke(this as T);
        }
    }
}