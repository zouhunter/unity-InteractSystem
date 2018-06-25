using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public sealed class PlaceCtrl : OperateController
    {
        public IHighLightItems highLight;
        private PickUpController pickCtrl { get { return PickUpController.Instence; } }
        public PlaceElement pickedUpObj
        {
            get
            {
                return pickCtrl.pickedUpObj.GetComponentInParent<PlaceElement>();
            }
        }
        public PlaceItem installPos;
        public bool installAble;
        public string resonwhy;
        public bool activeNotice { get { return Config.Instence.highLightNotice; } }
        public float hitDistence { get { return Config.Instence.hitDistence; } }

        private int _placePosLayerMask = 0;
        public int PlacePoslayerMask
        {
            get
            {
                if (_placePosLayerMask == 0)
                    _placePosLayerMask = LayerMask.GetMask(Layers.placePosLayer);
                return _placePosLayerMask;
            }
        }

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

        private void OnPickStay(PickUpAbleComponent arg0)
        {
            if (!Active) return;

            var placeElement = arg0.GetComponentInParent<PlaceElement>();
            if (placeElement)
            {
                TryPlaceObject(placeElement);
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
                var hitedObj = false;
                if (hits != null || hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        installPos = hits[i].collider.GetComponentInParent<PlaceItem>();
                        if (installPos)
                        {
                            hitedObj = true;
                            installAble = CanPlace(installPos, pickedUpObj, out resonwhy);
                            if (installAble)
                            {
                                break;
                            }
                        }
                    }
                }
                if (!hitedObj)
                {
                    installAble = false;
                    resonwhy = "零件放置位置不正确";
                }
            }

            if (installAble)
            {
                //可安装显示绿色
                if (activeNotice) highLight.HighLightTarget(pickedUpObj.ViewObj, Color.green);
            }
            else
            {
                //不可安装红色
                if (activeNotice) highLight.HighLightTarget(pickedUpObj.ViewObj, Color.red);
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
                highLight.UnHighLightTarget(pickedObj.ViewObj);
            }
        }

        public void PlaceObject(PlaceItem pos, PlaceElement pickup)
        {
            pos.PlaceObject(pickup);
        }

        public bool CanPlace(PlaceItem pos, PlaceElement element, out string why)
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
                pickup.RetriveFeature<PickUpAbleFeature>().OnPickDown();
            }
        }
        #endregion
    }
}
