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
        public UnityAction<BtnObj> onBtnClicked;
        public UnityAction<BtnObj> onHoverBtn;
        public UnityAction OnHoverNothing;

        private RaycastHit hit;
        private Ray ray;
        private BtnObj hitObj;
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

                yield return new WaitForFixedUpdate();
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
            }
        }

        private bool TryHitBtnObj(out BtnObj obj)
        {
            if (Physics.Raycast(ray,out hit,distence,LayerMask.GetMask(Setting.clickItemLayer)))
            {
                obj = hit.collider.GetComponent<BtnObj>();
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
