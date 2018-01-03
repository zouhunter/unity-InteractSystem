using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public class PickUpController
    {
        internal IPickUpAbleItem pickedUpObj { get; private set; }
        public bool PickedUp { get { return pickedUpObj != null; } }
        private Ray ray;
        private RaycastHit hit;
        private float hitDistence { get { return Config.hitDistence; } }
        private Ray disRay;
        private RaycastHit disHit;
        public float elementDistence { get; private set; }
        private const float minDistence = 1f;

        //private ActionSystem actionSystem;

        protected Camera viewCamera
        {
            get
            {
                return ActionSystem.Instence.cameraCtrl.currentCamera;
            }
        }

        private event UnityAction<IPickUpAbleItem> onPickup;
        private float timer = 0f;
        public PickUpController(/*ActionSystem actionSystem*/)
        {
            //this.actionSystem = actionSystem;
        }

        public void Update()
        {
            if (LeftTriggered())
            {
                if(HaveExecuteTwicePerSecond(ref timer))
                {
                    PickDown();
                }
                else if (!PickedUp)
                {
                    SelectAnElement();
                }
            }

            if (PickedUp)
            {
                elementDistence += Input.GetAxis("Mouse ScrollWheel");
                MoveWithMouse();
            }

            if (elementDistence < minDistence)
            {
                elementDistence = minDistence;
            }
        }


        internal void PickUp(IPickUpAbleItem pickedUpObj)
        {
            if (pickedUpObj != null)
            {
                this.pickedUpObj = pickedUpObj;
                pickedUpObj.OnPickUp();
                if (this.onPickup != null) onPickup.Invoke(pickedUpObj);
                elementDistence = Vector3.Distance(viewCamera.transform.position, pickedUpObj.Collider.transform.position);
            }

        }

        public void RegistOnPickup(UnityAction<IPickUpAbleItem> action)
        {
            onPickup += action;
        }

        public void PickDown()
        {
            if (pickedUpObj != null)
            {
                pickedUpObj.OnPickDown();
                pickedUpObj = null;
            }
        }
        public static bool HaveExecuteTwicePerSecond(ref float timer)
        {
            if (Time.time - timer < 0.5f)
            {
                return true;
            }
            else
            {
                timer = Time.time;
                return false;
            }
        }
        private bool LeftTriggered()
        {
            return Input.GetMouseButtonDown(0);
        }

        private bool CenterTriggered()
        {
            return Input.GetMouseButtonDown(2);
        }

        /// <summary>
        /// 跟随鼠标
        /// </summary>
        private void MoveWithMouse()
        {
            disRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(disRay, out disHit, elementDistence, 1 << Layers.obstacleLayer | 1 << Layers.placePosLayer))
            {
                pickedUpObj.SetPosition(GetPositionFromHit());
            }
            else
            {
                pickedUpObj.SetPosition( viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, elementDistence)));
            }
        }
        /// <summary>
        /// 利用射线获取对象移动坐标
        /// </summary>
        /// <returns></returns>
        private Vector3 GetPositionFromHit()
        {
            var normalPos = disHit.point;
            var boundPos = normalPos;
#if UNITY_5_6_OR_NEWER
            boundPos = pickedUpObj.Collider.ClosestPoint(normalPos);
#endif
            var centerPos = pickedUpObj.Collider.transform.position;
            var project = Vector3.Project(centerPos - boundPos, disRay.direction);
            var targetPos = normalPos - project;
            elementDistence -= Vector3.Distance(targetPos, pickedUpObj.Collider.transform.position);
            return targetPos;
        }

        /// <summary>
        /// 在未屏幕锁的情况下选中一个没有元素
        /// </summary>
       
        private void SelectAnElement()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, hitDistence, (1 << Layers.pickUpElementLayer)))
            {
                var pickedUpObj = hit.collider.GetComponent<PickUpAbleElement>();
                if(pickedUpObj.PickUpAble) PickUp(pickedUpObj);
            }
        }
    }
}
