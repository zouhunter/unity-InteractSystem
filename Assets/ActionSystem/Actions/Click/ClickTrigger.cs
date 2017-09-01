using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ClickTrigger : ActionTrigger
    {
        public bool hightButton;
        private Dictionary<string, List<BtnObj>> objDic = new Dictionary<string, List<BtnObj>>();
        private ClickContrller clickCtrl;
        private IHighLightItems highter;
        private string currStepName;
        private Renderer lastSelected;
        private List<int> queueID = new List<int>();
       protected override void Awake()
        {
            base.Awake();
            highter = new ShaderHighLight();
            if (hightButton) highter.SetState(hightButton);

            clickCtrl = new ClickContrller();
            clickCtrl.onBtnClicked = OnBtnClicked;
            clickCtrl.onHoverBtn = OnHoverBtn;
            clickCtrl.OnHoverNothing = OnHoverNothing;

            InitBtnObjs();
            StartCoroutine(clickCtrl.StartController());
        }

        public override IActionCommand CreateCommand()
        {
            return new ClickCommand(StepName, this);
        }

        internal void SetHighLightState(bool on)
        {
            highter.SetState(on);
        }

        void InitBtnObjs()
        {
            foreach (BtnObj obj in actionObjs)
            {
                if (objDic.ContainsKey((string)obj.StepName))
                {
                    objDic[(string)obj.StepName].Add((BtnObj)obj);
                }
                else
                {
                    objDic[(string)obj.StepName] = new System.Collections.Generic.List<BtnObj>() { obj };
                }
            }
        }

        void OnBtnClicked(BtnObj obj)
        {
            if (obj.Started && !obj.Complete)
            {
                obj.EndExecute();
                if (!SetNextButtonsClickAble())
                {
                    onStepComplete.Invoke(StepName);
                }
            }
        }
        void OnHoverBtn(BtnObj obj)
        {
            if (obj == null) return;
            if (lastSelected == obj.render) return;
            OnHoverNothing();
            highter.HighLightTarget(obj.render, obj.Started&&!obj.Complete ? Color.green : Color.red);
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

        internal void SetButtonClickAbleQueue(string stepName)
        {
            this.currStepName = stepName;
            queueID.Clear();
            var btns = objDic[stepName];
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
                    item.StartExecute();
                }
                return true;
            }
            return false;
        }

        internal void SetAllButtonUnClickAble(string stepName)
        {
            var btns = objDic[stepName];
            foreach (var item in btns)
            {
                item.UnDoExecute();
            }
        }


        internal void SetAllButtonClicked(string stepName,bool playAnim)
        {
            var btns = objDic[stepName];
            foreach (var item in btns){
                item.EndExecute();
            }
            if(playAnim) onStepComplete.Invoke(StepName);
        }

        internal void SetButtonNotClicked(string stepName)
        {
            var btns = objDic[stepName];
            foreach (var item in btns)
            {
                item.UnDoExecute();
            }
        }

       
    }

}
