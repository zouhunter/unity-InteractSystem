using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace WorldActionSystem
{
    public class ClickContrller : ICoroutineCtrl
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
        private IHighLightItems highLight;
        private List<int> queueID = new List<int>();
        private Renderer lastSelected;
        private ClickObj[] actionObjs;
        private ActionCommand trigger { get; set; }

        public ClickContrller(ActionCommand trigger, Camera camera, bool highLighter,ClickObj[] actionObjs)
        {
            this.actionObjs = actionObjs;
            viewCamera = camera;
            if (highLighter) highLight = new ShaderHighLight();
            InitCommand(trigger);
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
                obj.OnEndExecute();
                if (!SetNextButtonsClickAble())
                {
                    trigger.Complete();
                }
            }

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



        public IEnumerator Update()
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
                        if (onBtnClicked != null) onBtnClicked.Invoke(hitObj);
                    }
                    if (onHoverBtn != null) onHoverBtn.Invoke(hitObj);
                }
                else
                {
                    if (onHoverNothing != null) onHoverNothing.Invoke();
                    if (Input.GetMouseButtonDown(0) && EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
                    {
                        if (onClickEmpty != null) onClickEmpty.Invoke();
                    }
                }
                yield return null;
            }
        }

        internal void SetButtonClickAbleQueue()
        {
            queueID.Clear();
            foreach (ClickObj item in actionObjs)
            {
                if (!queueID.Contains(item.queueID))
                {
                    queueID.Add(item.queueID);
                }
            }
            queueID.Sort();
            SetNextButtonsClickAble();
        }

        private bool SetNextButtonsClickAble()
        {
            if (queueID.Count > 0)
            {
                var id = queueID[0];
                queueID.RemoveAt(0);
                var neetActive = Array.FindAll<ActionObj>(actionObjs, x => (x as ClickObj).queueID == id);
                foreach (var item in neetActive)
                {
                    item.OnStartExecute();
                }
                return true;
            }
            return false;
        }

        internal void SetAllButtonUnClickAble()
        {
            foreach (ClickObj item in actionObjs)
            {
                item.OnUnDoExecute();
            }
        }


        internal void SetAllButtonClicked(bool @continue)
        {
            foreach (ClickObj item in actionObjs)
            {
                item.OnEndExecute();
            }
            if (@continue)
            {
                trigger.Complete();
            }
        }

        internal void SetButtonNotClicked()
        {
            foreach (ClickObj item in actionObjs)
            {
                item.OnUnDoExecute();
            }
        }

        public void StartExecute(bool forceAuto)
        {
            if (forceAuto)
            {
                SetAllButtonClicked(true);
            }
            else
            {
                SetButtonClickAbleQueue();
            }
        }

        public void EndExecute()
        {
            SetAllButtonClicked(false);
        }

        public void UnDoExecute()
        {
            SetAllButtonUnClickAble();
            SetButtonNotClicked();
        }

        public void InitCommand(ActionCommand trigger)
        {
            this.trigger = trigger;
        }
    }

}
