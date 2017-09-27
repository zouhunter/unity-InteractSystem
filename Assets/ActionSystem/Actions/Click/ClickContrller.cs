using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace WorldActionSystem
{
    
    public class ClickContrller
    {
        public UnityAction<ClickObj> onBtnClicked;
        public UnityAction<ClickObj> onHoverBtn;
        public UnityAction onHoverNothing;
        public UnityAction onClickEmpty;

        private RaycastHit hit;
        private Ray ray;
        private ClickObj hitObj;
        private Vector3 screenPoint;
        private float distence = 10;
        private Camera viewCamera;
        public IEnumerator StartController(Camera camera)
        {
            viewCamera = camera??Camera.main;
            screenPoint = new Vector3();
            while (true)
            {
                screenPoint.x = Input.mousePosition.x;
                screenPoint.y = Input.mousePosition.y;
                screenPoint.z = 10;
                ray = viewCamera.ScreenPointToRay(screenPoint);

                if (TryHitBtnObj(out hitObj))
                {
                    if (Input.GetMouseButtonDown(0)){
                        if (onBtnClicked != null) onBtnClicked.Invoke(hitObj);
                    }
                    if (onHoverBtn != null) onHoverBtn.Invoke(hitObj);
                }
                else
                {
                    if (onHoverNothing != null) onHoverNothing.Invoke();
                    if (Input.GetMouseButtonDown(0) && EventSystem.current != null &&!EventSystem.current.IsPointerOverGameObject())
                    {
                        if(onClickEmpty != null) onClickEmpty.Invoke();
                    }
                }
                yield return null;
            }
        }

        private bool TryHitBtnObj(out ClickObj obj)
        {
            if (Physics.Raycast(ray,out hit,distence,(1<<Setting.clickItemLayer)))
            {
                obj = hit.collider.GetComponent<ClickObj>();
                return true;
            }
            obj = null;
            return false;
        }
    }

}
