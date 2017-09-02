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
        private ClickContrller clickCtrl;
        private IHighLightItems highter;
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
            highter.HighLightTarget(obj.render, obj.Started && !obj.Complete ? Color.green : Color.red);
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
            queueID.Clear();
            foreach (BtnObj item in actionObjs)
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
                var neetActive = Array.FindAll<ActionObj>(actionObjs, x => (x as BtnObj).queueID == id);
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
            foreach (BtnObj item in actionObjs)
            {
                item.UnDoExecute();
            }
        }


        internal void SetAllButtonClicked(string stepName, bool @continue)
        {
            foreach (BtnObj item in actionObjs)
            {
                item.EndExecute();
            }
            if (@continue) onStepComplete.Invoke(StepName);
        }

        internal void SetButtonNotClicked(string stepName)
        {
            foreach (BtnObj item in actionObjs)
            {
                item.UnDoExecute();
            }
        }


    }

}