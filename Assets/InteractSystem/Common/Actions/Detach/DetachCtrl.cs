using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Common.Actions
{
    public class DetachCtrl : OperateController
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Detach;
            }
        }
        protected PickUpController pickCtrl { get { return PickUpController.Instence; } }
        private DetachItem detachItem;

        public DetachCtrl()
        {
            pickCtrl.onPickup += OnPickUpElement;
            pickCtrl.onPickStay += OnPickStayElement;
            pickCtrl.onPickdown += OnPickDownElement;
            pickCtrl.onPickup += OnPickUpElement;
        }

        private void OnPickStayElement(PickUpAbleItem arg0)
        {
            var detachItem = arg0.GetComponentInParent<DetachItem>();
            DetachTargetItem(detachItem);
        }

        private void OnPickDownElement(PickUpAbleItem arg0)
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
        private void OnPickUpElement(PickUpAbleItem arg0)
        {
            var detachItem = arg0.GetComponentInParent<DetachItem>();
            if (detachItem)
            {
                Debug.Log("PickUp:" + arg0);
            }
        }

        public override void Update()
        {

        }

    }
}