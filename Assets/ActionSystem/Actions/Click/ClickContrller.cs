using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace WorldActionSystem
{
    public class ClickContrller : ActionCtroller
    {
        private RaycastHit hit;
        private Ray ray;
        private ClickObj hitObj;
        private Vector3 screenPoint;
        private float distence = 10;
        private Camera viewCamera;
        private IHighLightItems highLight;
        private Renderer lastSelected;

        public ClickContrller(ActionCommand trigger) : base(trigger)
        {
            viewCamera = trigger.viewCamera;
            highLight = new ShaderHighLight();
        }

        void OnBtnClicked(ClickObj obj)
        {
            if (!obj.Started)
            {
                trigger.UserError("不可点击" + obj.name);
            }
            else if (obj.Complete)
            {
                trigger.UserError("已经结束点击" + obj.name);
            }
            if (obj.Started && !obj.Complete)
            {
                obj.TryEndExecute();
            }
            highLight.UnHighLightTarget(obj.render);
        }

        void OnHoverBtn(ClickObj obj)
        {
            if (obj == null) return;
            if (lastSelected == obj.render) return;
            OnHoverNothing();
            highLight.HighLightTarget(obj.render, obj.Started && !obj.Complete ? Color.green : Color.red);
            lastSelected = obj.render;
        }

        void OnHoverNothing()
        {
            if (lastSelected != null)
            {
                highLight.UnHighLightTarget(lastSelected);
                lastSelected = null;
            }
        }

        void OnClickEmpty()
        {
            trigger.UserError("点击位置不正确");
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

        public override IEnumerator Update()
        {
            screenPoint = new Vector3();
            while (true)
            {
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
                yield return null;
            }
        }
    }

}
