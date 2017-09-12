using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ClickCommand : IActionCommand
    {
        private ClickTrigger trigger;
        public string StepName { get; private set; }

        public int Count { get; private set; }


        public ClickCommand(string stepName,int count, ClickTrigger trigger)
        {
            this.StepName = stepName;
            this.trigger = trigger;
            this.Count = count;
        }
        public  void StartExecute(bool forceAuto)
        {
            if (forceAuto) trigger.SetAllButtonClicked(StepName,true);
            else
            {
                trigger.SetButtonClickAbleQueue(StepName);
            }
        }
        public  void EndExecute()
        {
            trigger.SetAllButtonClicked(StepName,false);
        }
        public  void UnDoExecute()
        {
            trigger.SetAllButtonUnClickAble(StepName);
            trigger.SetButtonNotClicked(StepName);
        }
    }

}