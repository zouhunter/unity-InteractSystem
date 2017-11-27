using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace WorldActionSystem
{
    public class ClickContrller: IActionCtroller
    {
        public ControllerType CtrlType
        {
            get
            {
                return ControllerType.Click;
            }
        }
        private RaycastHit hit;
        private Ray ray;
        private ClickObj hitObj;
        private Vector3 screenPoint;
        private float distence = 10;
        private Camera viewCamera { get { return CameraController.ActiveCamera; } }
        private GameObject lastSelected;

        public UnityAction<string> UserError { get; set; }

        void OnBtnClicked(ClickObj obj)
        {
            if (!obj.Started)
            {
                UserError("不可点击" + obj.Name);
            }
            else if (obj.Complete)
            {
                UserError("已经结束点击" + obj.Name);
            }
            if (obj.Started && !obj.Complete)
            {
                obj.OnEndExecute(false);
            }
        }

        void OnHoverBtn(ClickObj obj)
        {
            if (obj == null) return;
            OnHoverNothing();
        }

        void OnHoverNothing()
        {
            if (lastSelected != null)
            {
                lastSelected = null;
            }
        }

        void OnClickEmpty()
        {
            UserError("点击位置不正确");
        }

        private bool TryHitBtnObj(out ClickObj obj)
        {
            if (Physics.Raycast(ray, out hit, distence, (1 << Setting.clickItemLayer)))
            {
                obj = hit.collider.GetComponent<ClickObj>();
                return true;
            }
            obj = null;
            return false;
        }

        public void Update()
        {
            screenPoint = new Vector3();

            screenPoint.x = Input.mousePosition.x;
            screenPoint.y = Input.mousePosition.y;
            screenPoint.z = 10;
            ray = viewCamera.ScreenPointToRay(screenPoint);

            if (TryHitBtnObj(out hitObj))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnBtnClicked(hitObj);
                }
                OnHoverBtn(hitObj);
            }
            else
            {
                OnHoverNothing();
                if (Input.GetMouseButtonDown(0) && EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
                {
                    OnClickEmpty();
                }
            }
        }

        public void OnStartExecute(bool forceAuto)
        {
            //throw new NotImplementedException();
        }

        public void OnEndExecute()
        {
            //throw new NotImplementedException();
        }

        public void OnUnDoExecute()
        {
            //throw new NotImplementedException();
        }
    }

}
