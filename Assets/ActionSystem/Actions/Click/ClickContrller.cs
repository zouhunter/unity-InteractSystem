using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace WorldActionSystem
{
    public class ClickContrller : OperateController
    {
        public override ControllerType CtrlType
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
      
        private GameObject lastSelected;

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
            if (Physics.Raycast(ray, out hit, distence, (1 << Layers.clickItemLayer)))
            {
                var hited = hit.collider.GetComponent<ClickObj>();
                if(CanOperate(hited)){
                    obj = hited;
                    return true;
                }
            }
            obj = null;
            return false;
        }

        public override void Update()
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
    }

}
