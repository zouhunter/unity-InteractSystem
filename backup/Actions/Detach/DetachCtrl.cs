using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldActionSystem.Actions
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
        protected PickUpController pickCtrl { get { return ActionSystem.Instence.pickupCtrl; } }

        public DetachCtrl()
        {
            pickCtrl.onPickup += OnPickUpElement;
        }

        private void OnPickUpElement(PickUpAbleItem arg0)
        {
            if (arg0 is DetachItem)
            {
                Debug.Log("PickUp:" + arg0);
            }
        }

        public override void Update()
        {

        }
    }
}