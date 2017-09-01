using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ClickAnimCommand : IActionCommand
    {
        private AnimGroup animParent;
        private BtnGroup btnParent;
        public string StepName { get; private set; }
        public CommandExecute onBeforeExecute;

        public ClickAnimCommand(string stepName, BtnGroup btnParent, AnimGroup animParent)
        {
            this.StepName = stepName;
            this.btnParent = btnParent;
            this.animParent = animParent;
            btnParent.onAllButtonClicked = () => animParent.PlayAnim(StepName);
        }
        public  void StartExecute(bool forceAuto)
        {
            if (onBeforeExecute != null) onBeforeExecute.Invoke(StepName);
            if (forceAuto) btnParent.SetAllButtonClicked(StepName,true);
            else
            {
                btnParent.SetButtonClickAbleQueue(StepName);
            }
        }
        public  void EndExecute()
        {
            btnParent.SetAllButtonClicked(StepName,false);
            animParent.SetAnimEnd(StepName);
        }
        public  void UnDoExecute()
        {
            btnParent.SetAllButtonUnClickAble(StepName);
            btnParent.SetButtonNotClicked(StepName);
            animParent.SetAnimUnPlayed(StepName);
        }
    }

}