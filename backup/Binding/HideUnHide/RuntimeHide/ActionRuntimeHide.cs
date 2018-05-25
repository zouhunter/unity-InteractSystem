using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldActionSystem.Binding
{

    public class ActionRuntimeHide : ActionObjBinding
    {
        [SerializeField]
        private string[] needHides;
        [SerializeField]
        private bool activeOnStart;
        [SerializeField]
        private bool activeOnComplete;
        private ElementController elementCtrl { get { return ElementController.Instence; } }
        protected override void OnBeforeActive(bool forceAuto)
        {
            base.OnBeforeActive(forceAuto);
            SetElementVisiable(activeOnStart);
        }
        protected override void OnBeforeComplete(bool force)
        {
            base.OnBeforeComplete(force);
            SetElementVisiable(activeOnComplete);
        }
        private void SetElementVisiable(bool isVisiable)
        {
            foreach (var item in needHides)
            {
                var elements = elementCtrl.GetElements<ISupportElement>(item);
                if (elements != null)
                {
                    foreach (var element in elements)
                    {
                        element.SetVisible(isVisiable);
                    }
                }

            }
        }

    }
}
