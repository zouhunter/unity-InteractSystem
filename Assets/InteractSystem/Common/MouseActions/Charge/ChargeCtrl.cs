using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace InteractSystem.Actions
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
        private ChargeTool chargeTool
        {
            get
            {
                return pickCtrl.pickedUpObj != null ?
                    pickCtrl.pickedUpObj.GetComponent<ChargeTool>() : null;
            }
        }
        private ChargeItem chargeObj;
        private ChargeItem lastMatchChargeObj;
        private ChargeResource chargeResource;
        private ChargeResource lastMatchChargeResource;
        IHighLightItems highter;
        public ChargeCtrl()
        {
            highter = new ShaderHighLight();
            pickCtrl = PickUpController.Instence;
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
                        if (chargeObj != lastMatchChargeObj)
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
                        if (lastMatchChargeResource != null)
                        {
                            highter.UnHighLightTarget(lastMatchChargeResource.gameObject);
                            lastMatchChargeResource = null;
                        }
                    }
                }
            }
        }

        private void OnPickStay(PickUpAbleComponent item)
        {
            var chargeTool = item.GetComponentInParent<ChargeTool>();
            if (chargeTool)
            {
                var currTool = chargeTool;
                if (chargeResource != null)
                {
                    var value = Mathf.Min(currTool.capacity, chargeResource.current);
                    var type = chargeResource.type;
                    currTool.RetriveFeature<PickUpAbleFeature>().PickUpAble = false;
                    currTool.LoadData(chargeResource.transform.position, new ChargeData(type, value), () =>
                    {
                        currTool.RetriveFeature<PickUpAbleFeature>().PickUpAble = true;
                    });
                    chargeResource.Subtruct(value, () => { });

                    highter.UnHighLightTarget(chargeResource.gameObject);
                    lastMatchChargeResource = chargeResource = null;
                }
                else if (chargeObj != null)
                {
                    var data = currTool.data;
                    ChargeData worpData = chargeObj.JudgeLeft(data);
                    if (!string.IsNullOrEmpty(worpData.type))
                    {
                        currTool.RetriveFeature<PickUpAbleFeature>().PickUpAble = false;
                        currTool.OnCharge(chargeObj.transform.position, worpData.value, () => { currTool.RetriveFeature<PickUpAbleFeature>().PickUpAble = true; });
                        chargeObj.Charge(worpData, () => { chargeObj.JudgeComplete(); });
                    }
                    highter.UnHighLightTarget(chargeObj.gameObject);
                    lastMatchChargeObj = null;
                }
            }

        }
    }

}
