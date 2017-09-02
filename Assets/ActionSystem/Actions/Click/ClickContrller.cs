using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    
    public class ClickContrller
    {
        public UnityAction<ClickObj> onBtnClicked;
        public UnityAction<ClickObj> onHoverBtn;
        public UnityAction OnHoverNothing;

        private RaycastHit hit;
        private Ray ray;
        private ClickObj hitObj;
        private Vector3 screenPoint;
        private float distence = 10;
        public IEnumerator StartController()
        {
            screenPoint = new Vector3();
            while (true)
            {
                screenPoint.x = Input.mousePosition.x;
                screenPoint.y = Input.mousePosition.y;
                screenPoint.z = 10;
                ray = Camera.main.ScreenPointToRay(screenPoint);

                if (TryHitBtnObj(out hitObj))
                {
                    if (TryClickBtnObj()){
                        if (onBtnClicked != null) onBtnClicked.Invoke(hitObj);
                    }
                    if (onHoverBtn != null) onHoverBtn.Invoke(hitObj);
                }
                else
                {
                    if (OnHoverNothing != null) OnHoverNothing.Invoke();
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
        private bool TryClickBtnObj()
        {
            if (Input.GetMouseButtonDown(0))
            {
                return true;
            }
            return false;
        }
    }

}
