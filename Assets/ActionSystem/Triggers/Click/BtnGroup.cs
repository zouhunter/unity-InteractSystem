using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class BtnGroup : MonoBehaviour
    {
        private Dictionary<string, List<BtnObj>> objDic = new Dictionary<string, List<BtnObj>>();
        public UnityAction onAllButtonClicked;
        private ClickContrller clickCtrl;
        private IHighLightItems highter;
        private string currStepName;
        private Renderer lastSelected;
        private List<int> queueID = new List<int>();
        void Start()
        {
            highter = new ShaderHighLight();
            clickCtrl = new ClickContrller();
            clickCtrl.onBtnClicked = OnBtnClicked;
            clickCtrl.onHoverBtn = OnHoverBtn;
            clickCtrl.OnHoverNothing = OnHoverNothing;
            InitBtnObjs();
            StartCoroutine(clickCtrl.StartController());
        }

        internal void SetHighLightState(bool on)
        {
            highter.SetState(on);
        }

        void InitBtnObjs()
        {
            var btns = GetComponentsInChildren<BtnObj>();
            foreach (BtnObj obj in btns)
            {
                if (objDic.ContainsKey((string)obj.stapName))
                {
                    objDic[(string)obj.stapName].Add((BtnObj)obj);
                }
                else
                {
                    objDic[(string)obj.stapName] = new System.Collections.Generic.List<BtnObj>() { obj };
                }
            }
        }

        void OnBtnClicked(BtnObj obj)
        {
            if (obj.clickAble)
            {
                obj.SetClicked();
                if (!SetNextButtonsClickAble())
                {
                    Debug.Log(onAllButtonClicked);
                    onAllButtonClicked.Invoke();
                }
            }
        }
        void OnHoverBtn(BtnObj obj)
        {
            if (obj == null) return;
            if (lastSelected == obj.render) return;
            OnHoverNothing();
            highter.HighLightTarget(obj.render, obj.clickAble ? Color.green : Color.red);
            lastSelected = obj.render;
        }

        void OnHoverNothing()
        {
            if (lastSelected != null)
            {
                highter.UnHighLightTarget(lastSelected);
                lastSelected = null;
            }
        }

        internal void SetButtonClickAbleQueue(string stapName)
        {
            this.currStepName = stapName;
            queueID.Clear();
            var btns = objDic[stapName];
            foreach (var item in btns)
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
                var items = objDic[currStepName];
                var neetActive = items.FindAll(x => x.queueID == id);
                foreach (var item in neetActive)
                {
                    item.clickAble = true;
                }
                return true;
            }
            return false;
        }

        internal void SetAllButtonUnClickAble(string stapName)
        {
            var btns = objDic[stapName];
            foreach (var item in btns)
            {
                item.clickAble = false;
            }
        }


        internal void SetAllButtonClicked(string stapName,bool playAnim)
        {
            var btns = objDic[stapName];
            foreach (var item in btns)
            {
                item.SetClicked();
            }
            if(playAnim) onAllButtonClicked.Invoke();
        }

        internal void ActiveStep(string stapName)
        {
            this.currStepName = stapName;
        }

        internal void SetButtonNotClicked(string stapName)
        {
            var btns = objDic[stapName];
            foreach (var item in btns)
            {
                item.SetUnClicked();
                item.clickAble = false;
            }
        }
    }

}
