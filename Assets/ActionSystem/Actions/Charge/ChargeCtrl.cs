using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace WorldActionSystem
{
    public class ChargeCtrl : OperateController
    {
        private PickUpController pickCtrl { get { return ActionSystem.Instence.pickupCtrl; } }
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Charge;
            }
        }
        private ChargeTool chargeTool { get { return pickCtrl.pickedUpObj != null && pickCtrl.pickedUpObj is ChargeTool ? pickCtrl.pickedUpObj as ChargeTool : null; } }
        private ChargeResource chargeResource;
        private ChargeObj chargeObj;

        public override void Update()
        {
            if(chargeTool != null)
            {
                if (!chargeTool.charged)
                {
                    TrySwink();
                   
                }
                else
                {
                    TryCharge();
                }
            }
        }

        private void TrySwink()
        {
            if (FindResource())
            {
                pickCtrl.PickStay();
                //chargeTool.LoadData(chargeResource);
             }
        }

        private bool FindResource()
        {
            return false;
        }

        private void TryCharge()
        {

        }
    }
         
}
