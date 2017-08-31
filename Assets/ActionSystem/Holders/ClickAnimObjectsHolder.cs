using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ClickAnimObjectsHolder : ActionHolder
    {
        private AnimGroup animParent;
        private BtnGroup btnParent;
        private bool _registed;
        private string currStepName;
        public override bool Registed
        {
            get
            {
                return _registed;
            }
        }

        private void Awake()
        {
            animParent = GetComponentInChildren<AnimGroup>();
            btnParent = GetComponentInChildren<BtnGroup>();
            btnParent.onAllButtonClicked = PlayAnim;
            animParent.onAllElementInit = OnAllInstallPosInit;
        }

        public override void SetHighLight(bool on)
        {
            btnParent.SetHighLightState(on);
        }

        private void OnAllInstallPosInit(Dictionary<string, List<AnimObj>> dic)
        {
            foreach (var list in dic)
            {
                var cmd = new ClickAnimCommand(list.Key, btnParent, animParent);
                cmd.executeAction += ActiveStep;
                if (OnRegistCommand != null) OnRegistCommand(cmd);
                foreach (var obj in list.Value)
                {
                    obj.onEndPlay = OnEndPlay;
                }
            }
            _registed = true;
        }
        private void PlayAnim()
        {
            animParent.PlayAnim(currStepName);
        }

        private void OnEndPlay(AnimObj obj)
        {
            if (CurrStapComplete())
            {
                if (OnStepEnd != null)
                    OnStepEnd.Invoke(obj.stapName);
            }
        }
        private void ActiveStep(string StepName)
        {
            currStepName = StepName;
            btnParent.ActiveStep(currStepName);
        }
        public bool CurrStapComplete()
        {
            bool complete = true;
            var list = animParent.GetCurrAnims(currStepName);
            foreach (var item in list)
            {
                complete &= item.Complete;
            }
            return complete;
        }

    }
}

