using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    public class PickUpController
    {
        internal PickUpAbleComponent pickedUpObj { get;private set; }
        public bool PickedUp { get { return pickedUpObj != null; } }
        private Ray ray;
        private RaycastHit hit;
        private float hitDistence { get { return Config.Instence.hitDistence; } }
        private Ray disRay;
        private RaycastHit disHit;
        public static bool log = false;
        public float elementDistence { get; private set; }
        private const float minDistence = 1f;
        private int pickUpElementLayerMask { get { return LayerMask.GetMask(PickUpAbleItem.layer); } }
        private int obstacleLayerMask { get { return LayerMask.GetMask(Layers.obstacleLayer); } }
        protected Camera viewCamera
        {
            get
            {
                return CameraController.Instence.currentCamera;
            }
        }

        public event UnityAction<PickUpAbleComponent> onPickup;
        public event UnityAction<PickUpAbleComponent> onPickdown;
        public event UnityAction<PickUpAbleComponent> onPickStay;
        public event UnityAction<PickUpAbleComponent> onPickTwinceLeft;
        public event UnityAction<PickUpAbleComponent> onPickTwinceRight;
        private float timer = 0f;
        private static PickUpController _instence;
        public static PickUpController Instence
        {

            get
            {
                if (_instence == null)
                {
                    _instence = new PickUpController(ActionSystem.Instence);
                }
                return _instence;
            }
        }

        private PickUpController(MonoBehaviour holder)
        {
            holder.StartCoroutine(Update());
        }

        private IEnumerator Update()
        {
            while (true)
            {
                yield return null;

                if (LeftTriggered())
                {
                    if (HaveExecuteTwicePerSecond(ref timer))
                    {
                        if (log) Debug.Log("HaveExecuteTwicePerSecond:0");

                        if (PickedUp && onPickTwinceLeft != null)
                        {
                            onPickTwinceLeft.Invoke(pickedUpObj);
                        }
                    }
                    else if (!PickedUp)
                    {
                        SelectAnElement();
                    }
                    else
                    {
                        PickStay();
                    }
                }

                if (RightTriggered())
                {
                    if (HaveExecuteTwicePerSecond(ref timer))
                    {
                        Debug.Log("HaveExecuteTwicePerSecond:1");
                        if (PickedUp && onPickTwinceRight != null)
                        {
                            onPickTwinceRight.Invoke(pickedUpObj);
                        }
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
        }

        internal void PickUp(PickUpAbleComponent pickedUpObj)
        {
            if (pickedUpObj != null)
            {
                this.pickedUpObj = pickedUpObj;
                pickedUpObj.OnPickUp();
                if (this.onPickup != null)
                    onPickup.Invoke(pickedUpObj);
                elementDistence = Vector3.Distance(viewCamera.transform.position, pickedUpObj.Collider.transform.position);
            }
        }

        public void PickStay()
        {
            if (pickedUpObj != null)
            {
                if (log) Debug.Log("onPickStay:" + pickedUpObj);

                var obj = pickedUpObj;
                pickedUpObj = null;

                obj.OnPickStay();

                if (onPickStay != null)
                    onPickStay(obj);
            }
        }

        public void PickDown()
        {
            if (log) Debug.Log("PickDown");

            if (pickedUpObj != null)
            {
                var obj = pickedUpObj;
                pickedUpObj = null;

                obj.OnPickDown();

                if (onPickdown != null)
                    onPickdown(obj);
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
        private bool RightTriggered()
        {
            return Input.GetMouseButtonDown(1);
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

            if (Physics.Raycast(disRay, out disHit, elementDistence, obstacleLayerMask) && disHit.collider != pickedUpObj.Collider)
            {
                pickedUpObj.SetPosition(GetPositionFromHit());
            }
            else
            {
                pickedUpObj.SetPosition(viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, elementDistence)));
            }

            pickedUpObj.SetViewForward(Vector3.ProjectOnPlane(disRay.direction, Vector3.up));
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
            if (Physics.Raycast(ray, out hit, hitDistence, pickUpElementLayerMask))
            {
                var pickedUpObj = hit.collider.gameObject.GetComponentInParent<PickUpAbleComponent>();
                if (pickedUpObj != null)
                {
                    if (pickedUpObj.PickUpAble)
                        PickUp(pickedUpObj);
                }
            }
        }

    }
}
