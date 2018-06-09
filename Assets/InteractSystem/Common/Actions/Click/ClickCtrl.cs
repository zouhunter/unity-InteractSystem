using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace InteractSystem.Common.Actions
{
    public class ClickCtrl : OperateController
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
        private ClickItem hitObj;
        private Vector3 screenPoint;
        private float distence { get { return Config.hitDistence; } }
      
        private GameObject lastSelected;

        void OnBtnClicked(ClickItem obj)
        {
            if (!obj.Active)
            {
                SetUserErr("不可点击" + obj.Name);
            }
            else
            {
                obj.OnComplete();
            }
        }

        void OnHoverClickItem(ClickItem obj)
        {
            if (obj == null) return;
            OnHoverNothing();
        }

        void OnHoverNothing()
        {
            if (lastSelected != null) {
                lastSelected = null;
            }
        }

        void OnClickEmpty()
        {
            SetUserErr("点击位置不正确");
        }

        private bool TryHitClickObj(out ClickItem obj)
        {
            if (Physics.Raycast(ray, out hit, distence,LayerMask.GetMask(Layers.clickItemLayer)))
            {
                obj = hit.collider.GetComponentInParent<ClickItem>();
                return true;
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

            if (TryHitClickObj(out hitObj))
            {
                if (Input.GetMouseButtonDown(0) && !HoverUI())
                {
                    OnBtnClicked(hitObj);
                }
                OnHoverClickItem(hitObj);
            }
            else
            {
                OnHoverNothing();

                if (Input.GetMouseButtonDown(0) && !HoverUI())
                {
                    OnClickEmpty();
                }
            }
        }

        private bool HoverUI()
        {
            return EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject();
        }
    }

}
