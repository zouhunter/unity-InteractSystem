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
            clickCtrl.onHoverNothing = OnHoverNothing;
            clickCtrl.onClickEmpty = OnClickEmpty;

            StartCoroutine(clickCtrl.StartController());
        }

        public override IList<IActionCommand> CreateCommands()
        {
            var cmds = new List<IActionCommand>();
            cmds.Add(new ClickCommand(StepName, this));
            return cmds;
        }

        internal void SetHighLightState(bool on)
        {
            highter.SetState(on);
        }
        void OnBtnClicked(ClickObj obj)
        {
            if (!obj.Started)
            {
                OnUserError("不可点击" + obj.name);
            }
            else if(obj.Complete)
            {
                OnUserError("已经结束点击" + obj.name);
            }
            if (obj.Started && !obj.Complete)
            {
                obj.EndExecute();
                if (!SetNextButtonsClickAble())
                {
                    OnComplete();
                }
            }

        }
        void OnHoverBtn(ClickObj obj)
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

        void OnClickEmpty()
        {
            OnUserError("点击位置不正确");
        }

        internal void SetButtonClickAbleQueue(string stepName)
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
                    item.StartExecute();
                }
                return true;
            }
            return false;
        }

        internal void SetAllButtonUnClickAble(string stepName)
        {
            foreach (ClickObj item in actionObjs)
            {
                item.UnDoExecute();
            }
        }


        internal void SetAllButtonClicked(string stepName, bool @continue)
        {
            foreach (ClickObj item in actionObjs)
            {
                item.EndExecute();
            }
            if (@continue)
            {
                OnComplete();
            }
        }

        internal void SetButtonNotClicked(string stepName)
        {
            foreach (ClickObj item in actionObjs)
            {
                item.UnDoExecute();
            }
        }


    }

}