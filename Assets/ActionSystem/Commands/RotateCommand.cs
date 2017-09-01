using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class RotateCommand : IActionCommand
    {
        private AnimGroup animParent;
        private RotGroup rotParent;
        public string StepName { get; private set; }
        public CommandExecute onBeforeExecute;


        public RotateCommand(string stepName, RotGroup rotParent, AnimGroup animParent) 
        {
            this.StepName = stepName;
            this.rotParent = rotParent;
            this.animParent = animParent;
        }
        public  void StartExecute(bool forceAuto)
        {
            if (onBeforeExecute != null) onBeforeExecute.Invoke(StepName);
            rotParent.ActiveStep(StepName);
            if (forceAuto) {
                rotParent.SetRotateComplete(true);
            }
            else{
                rotParent.SetRotateQueue(StepName);
            }
        }
        public  void EndExecute()
        {
            rotParent.SetRotateComplete();
            animParent.SetAnimEnd(StepName);
        }
        public  void UnDoExecute()
        {
            rotParent.SetStepUnDo(StepName);
            animParent.SetAnimUnPlayed(StepName);
        }
    }

}
