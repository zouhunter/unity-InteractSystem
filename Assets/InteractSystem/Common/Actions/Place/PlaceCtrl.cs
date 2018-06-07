using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    public sealed class PlaceCtrl : OperateController
    {
        public IHighLightItems highLight;
        private PickUpController pickCtrl { get { return PickUpController.Instence; } }
        public PlaceElement pickedUpObj { get { return pickCtrl.pickedUpObj is PlaceElement ? pickCtrl.pickedUpObj as PlaceElement : null; } }
        public PlaceObj installPos;
        public bool installAble;
        public string resonwhy;
        public bool activeNotice { get { return Config.highLightNotice; } }
        public float hitDistence { get { return Config.hitDistence; } }

        private int _placePosLayerMask = 0;
        public int PlacePoslayerMask { get { if(_placePosLayerMask == 0) _placePosLayerMask = LayerMask.GetMask(Layers.placePosLayer); return _placePosLayerMask; } }

        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Place;
            }
        }

        public const float minDistence = 1f;
        public Ray ray;
        public RaycastHit hit;
        public RaycastHit[] hits;
      
        public PlaceCtrl()
        {
            highLight = new ShaderHighLight();
            pickCtrl.onPickStay += OnPickStay;
        }

        private void OnPickStay(PickUpAbleItem arg0)
        {
            if (arg0 is PlaceElement)
            {
                TryPlaceObject(arg0 as PlaceElement);
            }
        }

        #region 鼠标操作事件
        public override void Update()
        {
            if (pickCtrl.PickedUp)
            {
                UpdatePlaceState();
            }
        }

        public void UpdatePlaceState()
        {
            if (pickedUpObj == null) return;

            if (!pickedUpObj.Active)
            {
                resonwhy = "当前步骤无需该零件!";
                installAble = false;
            }
            else
            {
                ray = viewCamera.ScreenPointToRay(Input.mousePosition);
                hits = Physics.RaycastAll(ray, hitDistence, PlacePoslayerMask);
                if (hits != null || hits.Length > 0)
                {
                    var hitedObj = false;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        installPos = hits[i].collider.GetComponentInParent<PlaceObj>();
                        if (installPos)
                        {
                            hitedObj = true;
                            installAble = CanPlace(installPos, pickCtrl.pickedUpObj, out resonwhy);
                            if (installAble)
                            {
                                break;
                            }
                        }
                    }
                    if (!hitedObj)
                    {
                        installAble = false;
                        resonwhy = "零件放置位置不正确";
                    }
                }
            }

            if (installAble)
            {
                //可安装显示绿色
                if (activeNotice) highLight.HighLightTarget(pickedUpObj.Render, Color.green);
            }
            else
            {
                //不可安装红色
                if (activeNotice) highLight.HighLightTarget(pickedUpObj.Render, Color.red);
            }
        }
        /// <summary>
        /// 尝试安装元素
        /// </summary>
        void TryPlaceObject(PlaceElement pickedObj)
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (installAble)
            {
                PlaceObject(installPos, pickedObj);
            }
            else
            {
                PlaceWrong(pickedObj);
                SetUserErr(resonwhy);
            }

            installAble = false;

            if (activeNotice)
            {
                highLight.UnHighLightTarget(pickedObj.Render);
            }
        }

        public void PlaceObject(PlaceObj pos, PlaceElement pickup)
        {
            pos.PlaceObject(pickup);
        }

        public bool CanPlace(PlaceObj pos, PickUpAbleItem element, out string why)
        {
            if (pos == null)
            {
                Debug.LogError("");
                why = "【配制错误】:零件未挂PlaceObj脚本";
                return false;
            }
            else
            {
                return pos.CanPlace(element, out why);
            }
        }

        public void PlaceWrong(PlaceElement pickup)
        {
            if (pickup)
            {
                pickup.OnPickDown();
            }
        }
        #endregion
    }
}
