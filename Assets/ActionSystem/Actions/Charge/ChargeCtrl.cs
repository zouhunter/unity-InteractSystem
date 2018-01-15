using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace WorldActionSystem
{
    public class ChargeCtrl : OperateController
    {
        private PickUpController pickCtrl;
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Charge;
            }
        }
        private ChargeTool chargeTool { get { return pickCtrl.pickedUpObj != null && pickCtrl.pickedUpObj is ChargeTool ? pickCtrl.pickedUpObj as ChargeTool : null; } }
        private ChargeObj chargeObj;
        private ChargeObj lastMatchChargeObj;
        private ChargeResource chargeResource;
        private ChargeResource lastMatchChargeResource;
        IHighLightItems highter;
        public ChargeCtrl()
        {
            highter = new ShaderHighLight();
            pickCtrl = ActionSystem.Instence.pickupCtrl;
            pickCtrl.onPickStay += OnPickStay;
        }
        public override void Update()
        {
            if (chargeTool != null)
            {
                if (chargeTool.charged)
                {
                    if (ChargeUtil.FindChargeObj(chargeTool, out chargeObj))
                    {
                        Debug.Log(chargeObj);
                        if(chargeObj != lastMatchChargeObj)
                        {
                            if (lastMatchChargeObj != null)
                            {
                                highter.UnHighLightTarget(lastMatchChargeObj.gameObject);
                            }
                            highter.HighLightTarget(chargeObj.gameObject, Color.green);
                            lastMatchChargeObj = chargeObj;
                        }
                    }
                    else
                    {
                        if (lastMatchChargeObj != null)
                        {
                            highter.UnHighLightTarget(lastMatchChargeObj.gameObject);
                            lastMatchChargeObj = null;
                        }
                    }
                }
                else
                {
                    if (ChargeUtil.FindResource(chargeTool, out chargeResource))
                    {
                        Debug.Log(chargeResource);
                        if (chargeResource != lastMatchChargeResource)
                        {
                            if (lastMatchChargeResource != null)
                            {
                                highter.UnHighLightTarget(lastMatchChargeResource.gameObject);
                            }
                            highter.HighLightTarget(chargeResource.gameObject, Color.green);
                            lastMatchChargeResource = chargeResource;
                        }
                    }
                    else
                    {
                        if(lastMatchChargeResource != null)
                        {
                            highter.UnHighLightTarget(lastMatchChargeResource.gameObject);
                            lastMatchChargeResource = null;
                        }
                    }
                }
            }
        }

        private void OnPickStay(PickUpAbleItem item)
        {
            if (item is ChargeTool)
            {
                if (chargeResource != null)
                {
                    var value = Mathf.Min(chargeTool.capacity, chargeResource.current);
                    var type = chargeResource.type;
                    chargeTool.LoadData(new ChargeData(type, value));
                    chargeResource.Subtruct(value);

                    highter.UnHighLightTarget(chargeResource.gameObject);
                    lastMatchChargeResource = chargeResource = null;
                }
                else if (chargeObj != null)
                {
                    var data = chargeTool.data;
                    float left = chargeObj.Charge(data);
                    if(left > 0){
                        data.value -= left;
                    }
                    chargeTool.OnCharge(data.value);
                    highter.UnHighLightTarget(chargeObj.gameObject);
                    lastMatchChargeObj = null;
                }
            }

        }
    }

}
