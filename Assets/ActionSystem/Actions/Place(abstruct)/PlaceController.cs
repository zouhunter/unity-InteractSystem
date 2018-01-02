using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{

    public abstract class PlaceController : OperateController
    {
        protected IHighLightItems highLight;
        protected PickUpAbleElement pickedUpObj;
        protected bool pickedUp;
        protected PlaceObj installPos;
        protected Ray ray;
        protected RaycastHit hit;
        protected RaycastHit[] hits;
        protected bool installAble;
        protected string resonwhy;
        protected float hitDistence { get { return Config.hitDistence; } }

        protected bool activeNotice { get { return Config.highLightNotice; } }
        protected Ray disRay;
        protected RaycastHit disHit;
        protected float elementDistence;
        protected abstract int PlacePoslayerMask { get; }//1 << Setting.installPosLayer
        protected const float minDistence = 1f;
        public PlaceController()
        {
            highLight = new ShaderHighLight();
        }
        #region 鼠标操作事件
        public override void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseClicked();
            }
            else if (pickedUp)
            {
                UpdatePlaceState();
                elementDistence += Input.GetAxis("Mouse ScrollWheel");
                MoveWithMouse();
            }

            if(elementDistence < minDistence)
            {
                elementDistence = minDistence;
            }
        }

        protected virtual void OnLeftMouseClicked()
        {
            if (!pickedUp)
            {
                SelectAnElement();
            }
            else
            {
                TryPlaceObject();
            }
        }

        /// <summary>
        /// 在未屏幕锁的情况下选中一个没有元素
        /// </summary>
        void SelectAnElement()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, hitDistence, (1 << Layers.pickUpElementLayer)))
            {
                var pickedUpObj = hit.collider.GetComponent<PickUpAbleElement>();
                if (pickedUpObj != null && !pickedUpObj.HaveBinding)
                {
                    this.pickedUpObj = pickedUpObj;
                    pickedUpObj.OnPickUp();
                    pickedUp = true;
                    OnSelectItem(pickedUpObj);
                    elementDistence = Vector3.Distance(viewCamera.transform.position, pickedUpObj.transform.position);
                }

            }
        }

        public void UpdatePlaceState()
        {
            if (!pickedUpObj.Started)
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
                        installPos = hits[i].collider.GetComponent<PlaceObj>();
                        if (installPos)
                        {
                            hitedObj = true;
                            installAble = CanPlace(installPos, pickedUpObj, out resonwhy);
                            if(installAble){
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
        void TryPlaceObject()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (installAble)
            {
                PlaceObject(installPos, pickedUpObj);
            }
            else
            {
                PlaceWrong(pickedUpObj);
                OnUserError(resonwhy);
            }

            pickedUp = false;
            installAble = false;
            if (activeNotice) highLight.UnHighLightTarget(pickedUpObj.Render);
        }

        protected abstract bool CanPlace(PlaceObj placeItem, PickUpAbleElement element, out string why);

        protected abstract void PlaceObject(PlaceObj pos, PickUpAbleElement pickup);

        protected abstract void PlaceWrong(PickUpAbleElement pickup);

        /// <summary>
        /// 跟随鼠标
        /// </summary>
        void MoveWithMouse()
        {
            disRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(disRay, out disHit, elementDistence, 1 << Layers.obstacleLayer | 1<< Layers.placePosLayer | 1<< Layers.matchPosLayer | 1<< Layers.installPosLayer))
            {
                pickedUpObj.transform.position = GetPositionFromHit();
            }
            else
            {
                pickedUpObj.transform.position = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, elementDistence));
            }
        }
        /// <summary>
        /// 利用射线获取对象移动坐标
        /// </summary>
        /// <returns></returns>
        Vector3 GetPositionFromHit()
        {
            var normalPos = disHit.point;
            var boundPos = normalPos;
#if UNITY_5_6_OR_NEWER
           boundPos = pickedUpObj.Collider.ClosestPoint(normalPos);
#endif
            var centerPos = pickedUpObj.transform.position;
            var project = Vector3.Project(centerPos - boundPos, disRay.direction);
            var targetPos = normalPos - project;
            elementDistence -= Vector3.Distance(targetPos,pickedUpObj.transform.position);
            return targetPos;
        }

        #endregion
    }
}
