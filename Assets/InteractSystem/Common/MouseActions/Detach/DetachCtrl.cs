using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Actions
{
    public class DetachCtrl:PCOperateCtrl<DetachCtrl>
    {
        protected PickUpController pickCtrl { get { return PickUpController.Instence; } }
        private DetachItem detachItem;

        public DetachCtrl()
        {
            pickCtrl.onPickup += OnPickUpElement;
            pickCtrl.onPickStay += OnPickStayElement;
            pickCtrl.onPickdown += OnPickDownElement;
        }

        private void OnPickStayElement(PickUpAbleComponent arg0)
        {
            var detachItem = arg0.GetComponentInParent<DetachItem>();
            DetachTargetItem(detachItem);
        }

        private void OnPickDownElement(PickUpAbleComponent arg0)
        {
            var detachItem = arg0.GetComponentInParent<DetachItem>();
            DetachTargetItem(detachItem);
        }
        private void DetachTargetItem(DetachItem detachItem)
        {
            if (detachItem)
            {
                detachItem.OnDetach();
            }
        }
        private void OnPickUpElement(PickUpAbleComponent arg0)
        {
            var detachItem = arg0.GetComponentInParent<DetachItem>();
            if (detachItem)
            {
                Debug.Log("PickUp:" + arg0);
                detachItem.UnNotice(detachItem.transform);
            }
        }
    }
}